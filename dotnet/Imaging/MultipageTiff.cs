static public void CreateMultipageTIFF(IList filenames, string tiffFilename)
{
	// set up the codec and parameters for the encoder
	ImageCodecInfo imageCodecInfo = null;
	foreach (ImageCodecInfo ini in ImageCodecInfo.GetImageEncoders())
		if (ini.MimeType == "image/tiff")
		{
			imageCodecInfo = ini;
			break;
		}
	if (imageCodecInfo == null)
		throw new Exception("An image encoder that support image/tiff could not be found");
	EncoderParameters ep = new EncoderParameters(1);
	ep.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);

	Image masterImage = Image.FromFile((string)filenames[0]);
	masterImage.Save(tiffFilename, imageCodecInfo, ep);
	ep.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
	for (int i = 1; i < filenames.Count; i++)
	{
		Image frame = Image.FromFile((string)filenames[i]);
		masterImage.SaveAdd(frame, ep);
		frame.Dispose();
		frame = null;
	}
	ep.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);
	masterImage.SaveAdd(ep);
	masterImage.Dispose();
	masterImage = null;
}

/// <summary>
/// Appends the image at src to the TIFF at dest
/// </summary>
/// <param name="src"></param>
/// <param name="dest"></param>
static public void AppendToMultipageTIFF(string src, string dest)
{
	// set up the codec and parameters for the encoder
	ImageCodecInfo imageCodecInfo = null;
	foreach (ImageCodecInfo ini in ImageCodecInfo.GetImageEncoders())
		if (ini.MimeType == "image/tiff")
		{
			imageCodecInfo = ini;
			break;
		}
	if (imageCodecInfo == null)
		throw new Exception("An image encoder that support image/tiff could not be found");
	EncoderParameters ep = new EncoderParameters(2);
	ep.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.MultiFrame);
	ep.Param[1] = new EncoderParameter(Encoder.Compression, (long)EncoderValue.CompressionCCITT4);

	// load up the existing destination image into memory (this leaves the dest file writable)
	Bitmap existingImage = null;
	FileStream fs = File.Open(dest, FileMode.Open, FileAccess.Read);
	existingImage = (Bitmap)Bitmap.FromStream(fs);

	// get the page count of the existing image and get a temp filename
	int existingPageCount = existingImage.GetFrameCount(FrameDimension.Page);
	string tmpFilename = PathSupport.GetTempFilename("multitiff", "tif");

	// start a new image using the first frame of the existing image
	existingImage.SelectActiveFrame(FrameDimension.Page, 0);
	Bitmap masterImage = ConvertToBitonal(new Bitmap(existingImage));
	masterImage.Save(tmpFilename, imageCodecInfo, ep);
	
	// save the following frames of the existing image
	ep.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.FrameDimensionPage);
	Bitmap frame = null;
	for (int i = 1; i < existingPageCount; i++)
	{
		existingImage.SelectActiveFrame(FrameDimension.Page, i);
		frame = ConvertToBitonal(new Bitmap(existingImage));
		masterImage.SaveAdd(frame, ep);
		frame.Dispose();
		frame = null;
	}
	
	// save the new file to the image
	frame = ConvertToBitonal(new Bitmap(src));
	masterImage.SaveAdd(frame, ep);
	frame.Dispose();
	frame = null;

	// flush the master image
	ep.Param[0] = new EncoderParameter(Encoder.SaveFlag, (long)EncoderValue.Flush);
	masterImage.SaveAdd(ep);
	masterImage.Dispose();
	masterImage = null;

	// move the new tiff over the old one
	fs.Close();
	File.Delete(dest);
	//File.Move(tmpFilename, dest);
	File.Copy(tmpFilename, dest, true);
	File.Delete(tmpFilename);
}

static private Bitmap ConvertToBitonal(Bitmap original)
{
	// Only convert if not already 1bpp
	if (original.PixelFormat == PixelFormat.Format1bppIndexed)
		return original;

	Bitmap source = null;

	// If original bitmap is not already in 32 BPP, ARGB format, then convert
	if (original.PixelFormat != PixelFormat.Format32bppArgb)
	{
		source = new Bitmap(original.Width, original.Height, PixelFormat.Format32bppArgb);
		source.SetResolution(original.HorizontalResolution, original.VerticalResolution);
		using (Graphics g = Graphics.FromImage(source))
		{
			g.DrawImageUnscaled(original, 0, 0);
		}
	}
	else
	{
		source = original;
	}

	// Lock source bitmap in memory
	BitmapData sourceData = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadOnly, PixelFormat.Format32bppArgb);

	// Copy image data to binary array
	int imageSize = sourceData.Stride * sourceData.Height;
	byte[] sourceBuffer = new byte[imageSize];
	Marshal.Copy(sourceData.Scan0, sourceBuffer, 0, imageSize);

	// Unlock source bitmap
	source.UnlockBits(sourceData);

	// Create destination bitmap
	Bitmap destination = new Bitmap(source.Width, source.Height, PixelFormat.Format1bppIndexed);

	// Lock destination bitmap in memory
	BitmapData destinationData = destination.LockBits(new Rectangle(0, 0, destination.Width, destination.Height), ImageLockMode.WriteOnly, PixelFormat.Format1bppIndexed);

	// Create destination buffer
	imageSize = destinationData.Stride * destinationData.Height;
	byte[] destinationBuffer = new byte[imageSize];

	int sourceIndex = 0;
	int destinationIndex = 0;
	int pixelTotal = 0;
	byte destinationValue = 0;
	int pixelValue = 128;
	int height = source.Height;
	int width = source.Width;
	int threshold = 500;

	// Iterate lines
	for (int y = 0; y < height; y++)
	{
		sourceIndex = y * sourceData.Stride;
		destinationIndex = y * destinationData.Stride;
		destinationValue = 0;
		pixelValue = 128;

		// Iterate pixels
		for (int x = 0; x < width; x++)
		{
			// Compute pixel brightness (i.e. total of Red, Green, and Blue values)
			pixelTotal = sourceBuffer[sourceIndex + 1] + sourceBuffer[sourceIndex + 2] + sourceBuffer[sourceIndex + 3];
			if (pixelTotal > threshold)
			{
				destinationValue += (byte)pixelValue;
			}
			if (pixelValue == 1)
			{
				destinationBuffer[destinationIndex] = destinationValue;
				destinationIndex++;
				destinationValue = 0;
				pixelValue = 128;
			}
			else
			{
				pixelValue >>= 1;
			}
			sourceIndex += 4;
		}
		if (pixelValue != 128)
		{
			destinationBuffer[destinationIndex] = destinationValue;
		}
	}

	// Copy binary image data to destination bitmap
	Marshal.Copy(destinationBuffer, 0, destinationData.Scan0, imageSize);

	// Unlock destination bitmap
	destination.UnlockBits(destinationData);

	// Return
	return destination;
}



