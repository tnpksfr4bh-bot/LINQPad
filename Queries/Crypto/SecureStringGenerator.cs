using System.Security.Cryptography;
using System.Text;

public static class SecureStringGenerator
{
	private const string AlphanumericChars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
	// Reduced charset avoids visually ambiguous characters (0/O, 1/l/I)
	private const string AppIdChars        = "acefghkrstxyz235678";

	public static string Generate(int length = 16)
	{
		var bytes = new byte[length];
		RandomNumberGenerator.Fill(bytes);
		var sb = new StringBuilder(length);
		foreach (var b in bytes)
			sb.Append(AlphanumericChars[b % AlphanumericChars.Length]);
		return sb.ToString();
	}

	public static string GenerateAppId(int length = 16)
	{
		var bytes = new byte[length];
		RandomNumberGenerator.Fill(bytes);
		var sb = new StringBuilder(length);
		foreach (var b in bytes)
			sb.Append(AppIdChars[b % AppIdChars.Length]);
		return sb.ToString();
	}
}
