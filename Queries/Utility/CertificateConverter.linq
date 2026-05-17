<Query Kind="Program">
  <Namespace>System.Security.Cryptography.X509Certificates</Namespace>
</Query>

// --- Test ---
void Main()
{
	// Encode a P12 file to Base64
	// string b64 = CertificateConverter.FileToBase64(@"C:\Temp\service.p12");
	// Console.WriteLine(b64);

	// Decode and load back
	// var cert = CertificateConverter.Base64ToCert(b64, "notasecret");
	// Console.WriteLine($"Subject: {cert.Subject}");
	// Console.WriteLine($"Thumbprint: {cert.Thumbprint}");

	Console.WriteLine("Set path above and uncomment to test.");
}

// ---

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
