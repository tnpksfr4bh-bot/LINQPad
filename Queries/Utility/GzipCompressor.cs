using System.IO.Compression;
using System.Text;

// UTF-8 text → GZip → Base64 and back.
public static class GzipCompressor
{
	public static string Compress(string text)
	{
		if (string.IsNullOrEmpty(text)) return text;
		byte[] bytes = Encoding.UTF8.GetBytes(text);
		using var ms  = new MemoryStream();
		using (var gz = new GZipStream(ms, CompressionMode.Compress))
		{
			gz.Write(bytes, 0, bytes.Length);
			gz.Flush();
		}
		return Convert.ToBase64String(ms.ToArray());
	}

	public static string Decompress(string base64)
	{
		if (string.IsNullOrEmpty(base64)) return base64;
		using var ms  = new MemoryStream(Convert.FromBase64String(base64));
		using var gz  = new GZipStream(ms, CompressionMode.Decompress);
		using var sr  = new StreamReader(gz, Encoding.UTF8);
		return sr.ReadToEnd();
	}

	// Returns (compressed, decompressed) — compressed is echoed back for logging
	public static (string compressed, string decompressed) DecompressWithRaw(string base64)
		=> (base64, Decompress(base64));
}
