<Query Kind="Program">
  <Namespace>System.Security.Cryptography.X509Certificates</Namespace>
</Query>

#load "CertificateConverter.cs"

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
