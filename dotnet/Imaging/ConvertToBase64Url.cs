public sealed class ConvertToBase64Url
{
	static public string FromFile(string filename)
	{
		byte[] buf = null;

		using (FileStream fs = File.OpenRead(filename))
		{
			buf = new byte[fs.Length];
			fs.Read(buf, 0, (int)fs.Length);
		}

		return FromRawBuffer(buf, filename);
	}
	static public string FromRawBuffer(byte[] buf, string filename)
	{
		string url = "data:";
		if (filename.ToLower().EndsWith("jpg") || filename.ToLower().EndsWith("jpeg"))
		{
			url += "image/jpeg";
		}
		else if (filename.ToLower().EndsWith("png"))
		{
			url += "image/png";
		}
		else if (filename.ToLower().EndsWith("gif"))
		{
			url += "image/gif";
		}
		else if (filename.ToLower().EndsWith("emf"))
		{
			url += "image/x-emf";
		}

		url += ";base64," + Convert.ToBase64String(buf);

		return url;
	}
}
