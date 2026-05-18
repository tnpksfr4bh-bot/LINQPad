using System.Security.Cryptography;
using System.Text;

public static class EcdsaCryptoService
{
	public static ECDsaCng CreateKey(int keySize = 256)
		=> new ECDsaCng(keySize) { HashAlgorithm = CngAlgorithm.Sha256 };

	public static string ExportPublicKey(ECDsaCng ecdsa)
		=> Convert.ToBase64String(ecdsa.Key.Export(CngKeyBlobFormat.EccPublicBlob));

	public static byte[] Sign(ECDsaCng ecdsa, string message)
		=> ecdsa.SignData(Encoding.UTF8.GetBytes(message));

	public static bool Verify(ECDsaCng ecdsa, string message, byte[] signature)
		=> ecdsa.VerifyData(Encoding.UTF8.GetBytes(message), signature);

	public static void RunTest()
	{
		using var ecdsa  = CreateKey();
		var publicKey    = ExportPublicKey(ecdsa);
		var message      = "ECDSA test message";
		var signature    = Sign(ecdsa, message);
		var verified     = Verify(ecdsa, message, signature);

		Console.WriteLine($"Public Key : {publicKey}");
		Console.WriteLine($"Signature  : {Convert.ToBase64String(signature)}");
		Console.WriteLine($"Verified   : {verified}");
	}
}
