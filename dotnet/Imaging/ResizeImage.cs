static public Image ResizeImage(Image src, int width, int height)
{
	int newWidth = width;
	int newHeight = height;
	if (src.Height < src.Width)
		newHeight = Convert.ToInt32((double)src.Height * ((double)newWidth/(double)src.Width));
	else if (src.Width < src.Height)
		newWidth = Convert.ToInt32((double)src.Width * ((double)newHeight/(double)src.Height));
	
	Bitmap dest = new Bitmap(src, newWidth, newHeight);
	
	return dest;
}
