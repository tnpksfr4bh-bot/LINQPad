using System.Security.Cryptography.X509Certificates;

// Converts certificate files (P12/PFX) to/from Base64 strings.
// Useful for embedding credentials in appsettings or environment variables.
//
// Build command (OpenSSL):
//   openssl pkcs12 -export -out cert.pfx -inkey private_key.txt -in cert.crt -certfile chain.crt
public static class CertificateConverter
{
	// Read a P12/PFX/PEM file and return Base64-encoded bytes
	public static string FileToBase64(string filePath)
	{
		byte[] bytes = File.ReadAllBytes(filePath);
		return Convert.ToBase64String(bytes);
	}

	// Decode Base64 and load as X509Certificate2 (password required for P12/PFX)
	public static X509Certificate2 Base64ToCert(string base64, string password = null,
		X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet)
	{
		byte[] bytes = Convert.FromBase64String(base64);
		return string.IsNullOrEmpty(password)
			? new X509Certificate2(bytes)
			: new X509Certificate2(bytes, password, flags);
	}

	// Load a P12/PFX file directly
	public static X509Certificate2 LoadFromFile(string filePath, string password = null,
		X509KeyStorageFlags flags = X509KeyStorageFlags.MachineKeySet)
	{
		byte[] bytes = File.ReadAllBytes(filePath);
		return string.IsNullOrEmpty(password)
			? new X509Certificate2(bytes)
			: new X509Certificate2(bytes, password, flags);
	}

	// Save certificate bytes to a file
	public static void Base64ToFile(string base64, string outputPath)
		=> File.WriteAllBytes(outputPath, Convert.FromBase64String(base64));
}
