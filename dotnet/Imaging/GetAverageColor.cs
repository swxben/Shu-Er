/* 
Get the average color (red, green, blue) from an image.
This was to be used in a ADF scanner utility that could detect a seperator page
of a certain colour.
*/

static public void GetAverageColor(string filename, ref int red, ref int green, ref int blue)
{
	Bitmap bmp = (Bitmap)Bitmap.FromFile(filename);
	GetAverageColor(bmp, ref red, ref green, ref blue);
	bmp.Dispose();
	bmp = null;
}
static public void GetAverageColor(Bitmap bmp, ref int red, ref int green, ref int blue)
{
	// This is probably way too accurate, it could maybe process every tenth scanline and still be as useful
	// (and take almost 1/10 as long)

	unsafe
	{
		BitmapData bmpData = bmp.LockBits(
			new Rectangle(0,0,bmp.Width,bmp.Height),
			ImageLockMode.ReadOnly,
			PixelFormat.Format24bppRgb);

		int bytesPerPixel = 3;
		int padding = bmpData.Stride - bmpData.Width * bytesPerPixel;	// padding at the end of each scan line
		byte* p = (byte*)bmpData.Scan0.ToPointer();

		int[] rr = new int[bmp.Height];
		int[] bb = new int[bmp.Height];
		int[] gg = new int[bmp.Height];

		for (int y = 0; y < bmp.Height; y++)
		{
			for(int x = 0; x < bmp.Width; x++)
			{
				rr[y] += (int)p[0];
				bb[y] += (int)p[1];
				gg[y] += (int)p[2];

				p += bytesPerPixel;
			}
			p += padding;
		}

		bmp.UnlockBits(bmpData);

		int r = 0; int g = 0; int b = 0;
		for (int i = 0; i < bmp.Height; i++)
		{
			r += rr[i]/bmp.Width;
			b += bb[i]/bmp.Width;
			g += gg[i]/bmp.Width;
		}

		int pixelCount = bmp.Height * bmp.Width;
		red = Convert.ToInt32((decimal)r / (decimal)bmp.Height);
		blue = Convert.ToInt32((decimal)b / (decimal)bmp.Height);
		green = Convert.ToInt32((decimal)g / (decimal)bmp.Height);
	}
}
