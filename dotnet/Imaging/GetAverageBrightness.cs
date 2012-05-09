/* These calculate the average brightness (0 <= (double)brightness <= 1).
The Approx variation samples a random selection of scanlines.
This is used in a scanning application to detect inverted (white -> black) pages. */

static private double GetAverageBrightness(Bitmap bmp)
{
	// This is probably way too accurate, it could maybe process every tenth scanline and still be as useful
	// (and take almost 1/10 as long)

	unsafe
	{
		BitmapData bmpData = bmp.LockBits(
			new Rectangle(0,0,bmp.Width,bmp.Height),
			ImageLockMode.ReadOnly,
			PixelFormat.Format24bppRgb);

		int padding = bmpData.Stride - bmpData.Width * 3;	// padding at the end of each scan line
		byte* p = (byte*)bmpData.Scan0.ToPointer();

		int[] values = new int[bmp.Height];

		for (int y = 0; y < bmp.Height; y++)
		{
			for(int x = 0; x < bmp.Width; x++)
			{
				values[y] += (int)p[0] + (int)p[1] + (int)p[2];

				p += 3;
			}
			p += padding;
		}

		bmp.UnlockBits(bmpData);

		double brightness = 0;
		for (int i = 0; i < bmp.Height; i++)
			brightness += (values[i]/3)/bmp.Width;

		brightness = brightness / bmp.Height;
		brightness = brightness / 255;

		return brightness;
	}
}

/// <summary>
/// returns a value between 0 and 1 where 0 is pure black and 1 is pure white, to a given approx accuracy (between 0 and 1).
/// The accuracy is only approx because it uses a random sample of scan lines to get the brightness, the size of the sample
/// is (accuracy * number of scan lines). If accuracy is 1, the exact average brightness is given using GetAverageBrightness().
/// </summary>
/// <param name="filename"></param>
/// <returns></returns>
static public double GetApproxAverageBrightness(string filename, double accuracy)
{
	Bitmap bmp = (Bitmap)Bitmap.FromFile(filename);
	double brightness = GetApproxAverageBrightness(bmp, accuracy);
	bmp.Dispose();
	bmp = null;

	return brightness;
}
static private double GetApproxAverageBrightness(Bitmap bmp, double accuracy)
{
	if (accuracy <= 0 || accuracy > 1)
		throw new ArgumentException("The accuracy must be greater than 0 and equal to or less than 1", "accuracy");

	if (accuracy == 1)
		return GetAverageBrightness(bmp);

	unsafe
	{
		BitmapData bmpData = bmp.LockBits(
			new Rectangle(0,0,bmp.Width,bmp.Height),
			ImageLockMode.ReadOnly,
			PixelFormat.Format24bppRgb);

		int padding = bmpData.Stride - bmpData.Width * 3;	// padding at the end of each scan line
		int height = bmp.Height;
		int width = bmp.Width;

		int[] values = new int[Convert.ToInt32(height * accuracy)];
		Random random = new Random();

		for (int i = 0; i < values.Length; i++)
		{
			int y = random.Next(height);
			byte* p = (byte*)bmpData.Scan0.ToPointer() + (y * bmpData.Stride);

			for(int x = 0; x < width; x++)
			{
				values[i] += (int)p[0] + (int)p[1] + (int)p[2];

				p += 3;
			}
		}

		bmp.UnlockBits(bmpData);

		double brightness = 0;
		for (int i = 0; i < values.Length; i++)
			brightness += (values[i]/3)/width;

		brightness /= values.Length;
		brightness /= 255;

		return brightness;
	}
}
