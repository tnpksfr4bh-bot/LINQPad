<Query Kind="Program">
  <NuGetReference>BouncyCastle.Cryptography</NuGetReference>
  <Namespace>Org.BouncyCastle.Crypto</Namespace>
  <Namespace>Org.BouncyCastle.Crypto.Parameters</Namespace>
  <Namespace>Org.BouncyCastle.OpenSsl</Namespace>
  <Namespace>Org.BouncyCastle.Security</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

#load "RsaCrypto.cs"

void Main()
{
	// Generate new key pair
	var (publicPem, privatePem) = RsaKeyConverter.GenerateKeyPair(1024);
	Console.WriteLine(publicPem);

	// Import and use
	var publicKey  = RsaKeyConverter.ImportPublicKey(publicPem);
	var privateKey = RsaKeyConverter.ImportPrivateKey(privatePem);

	var plainText = "aid|pid|sid|0";
	var encrypted = RsaCryptoService.Encrypt(Encoding.Unicode.GetBytes(plainText), publicKey,  doOaepPadding: false);
	var decrypted = RsaCryptoService.Decrypt(encrypted,                            privateKey, doOaepPadding: false);

	Console.WriteLine($"Decrypted: {Encoding.Unicode.GetString(decrypted)}");
}
