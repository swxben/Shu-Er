/*
Invert (colour) an image
*/

public static void InvertImage(string filename)
{
	// Get a temp filename, load and invert the source in memory, save to the temp file, dispose,
	// delete the source, copy the temp into the source filename and delete the temp.
	string tmpFilename = PathSupport.GetTempFilename("imagesupport_invertimage", Path.GetExtension(filename).Replace(".",""));
	Bitmap b = InvertImage((Bitmap)Bitmap.FromFile(filename));
	b.Save(tmpFilename);
	b.Dispose();
	b = null;
	File.Delete(filename);
	File.Copy(tmpFilename, filename, true);
	File.Delete(tmpFilename);
}

/// <summary>
/// This will only work for 1bpp index images, otherwise it returns the source. It does a bitwise inversion of 
/// the buffer. This could be optimised by directly accessing the buffer in an unsafe block (rather than using
/// Marshal.Copy() to work on a byte[]) but this seems pretty quick anyway.
/// </summary>
/// <param name="source"></param>
/// <returns></returns>
public static Bitmap InvertImage(Bitmap source)
{
	if (source.PixelFormat != PixelFormat.Format1bppIndexed)
	{
		return source;
	}

	// lock the bitmap data and get a copy of the image buffer
	BitmapData data = source.LockBits(new Rectangle(0, 0, source.Width, source.Height), ImageLockMode.ReadWrite, PixelFormat.Format1bppIndexed);
	byte[] buf = new byte[data.Height * data.Stride];
	Marshal.Copy(data.Scan0, buf, 0, buf.Length);

	// invert each byte of the buffer
	for (int i = 0; i < buf.Length; i++)
	{
		buf[i] = (byte)~buf[i];
	}

	// copy the inverted buffer back into the image
	Marshal.Copy(buf, 0, data.Scan0, buf.Length);

	// unlock bitmap data
	source.UnlockBits(data);

	// return
	return source;
}

/// <summary>
/// This method converts the image into a 24bpp bitmap first, then inverts it, then copies it into an image of the same
/// bpp as the source, so it should work for non-indexed bitmaps (untested)
/// </summary>
/// <param name="source"></param>
/// <returns></returns>
public static Bitmap InvertImageTrueColor(Bitmap source)
{
	// copy the source bitmap into a new 24bpp bitmap
	Bitmap copy = new Bitmap(source.Width, source.Height, PixelFormat.Format24bppRgb);
	copy.SetResolution(source.HorizontalResolution, source.VerticalResolution);
	using (Graphics g = Graphics.FromImage(copy))
	{
		g.DrawImageUnscaled(source, 0, 0);
	}

	// invert the image in the copy
	BitmapData data = copy.LockBits(new Rectangle(0, 0, copy.Width, copy.Height), ImageLockMode.ReadWrite, PixelFormat.Format24bppRgb);
	int stride = data.Stride;
	IntPtr scan0 = data.Scan0;
	unsafe
	{
		byte* p = (byte*)(void*)scan0;
		int offset = stride - copy.Width * 3;
		for (int y = 0; y < copy.Height; y++)
		{
			for (int x = 0; x < copy.Width; x++)
			{
				p[0] = (byte)(255 - p[0]);
				p[1] = (byte)(255 - p[1]);
				p[2] = (byte)(255 - p[2]);
				p += 3;
			}
			p += offset;
		}
	}
	copy.UnlockBits(data);

	// make a new dest bitmap of the same pixel format as the source and copy the inverted bitmap in
	Bitmap dest = new Bitmap(source.Width, source.Height, source.PixelFormat);
	dest.SetResolution(source.HorizontalResolution, source.VerticalResolution);
	using (Graphics g = Graphics.FromImage(dest))
	{
		g.DrawImageUnscaled(copy, 0, 0);
	}

	// dispose the source and copy bitmaps
	source.Dispose();
	source = null;
	copy.Dispose();
	copy = null;

	// return the dest bitmap
	return dest;
}


