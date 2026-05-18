using System.Security.Cryptography;
using System.Text;

public static class AesCryptoService
{
	public static string Encrypt(string text, string key, string iv, bool useUrlSafe = false)
	{
		if (text == null) return null;

		using var aes = Aes.Create();
		aes.Key = Encoding.UTF8.GetBytes(key);
		aes.IV  = Encoding.UTF8.GetBytes(iv);
		using var encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
		using var ms = new MemoryStream();
		using (var cs = new CryptoStream(ms, encryptor, CryptoStreamMode.Write))
		using (var sw = new StreamWriter(cs))
			sw.Write(text);

		return useUrlSafe
			? Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlEncode(ms.ToArray())
			: Convert.ToBase64String(ms.ToArray());
	}

	// Handles both standard Base64 and URL-safe Base64 (padding-tolerant)
	public static string Decrypt(string text, string key, string iv)
	{
		if (text == null)         return null;
		if (text == string.Empty) return string.Empty;

		using var aes = Aes.Create();
		aes.Key = Encoding.UTF8.GetBytes(key);
		aes.IV  = Encoding.UTF8.GetBytes(iv);
		var bytes = Microsoft.AspNetCore.WebUtilities.WebEncoders.Base64UrlDecode(text);
		using var decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
		using var ms = new MemoryStream(bytes);
		using var cs = new CryptoStream(ms, decryptor, CryptoStreamMode.Read);
		using var sr = new StreamReader(cs);
		return sr.ReadToEnd();
	}

	// byteLength → Base64 length table:
	//  8 → 12,  12 → 16,  16 → 24,  24 → 32
	public static string SecureRandom(int byteLength = 24)
	{
		var bytes = new byte[byteLength];
		RandomNumberGenerator.Fill(bytes);
		return Convert.ToBase64String(bytes);
	}
}
