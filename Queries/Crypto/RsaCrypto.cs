using Org.BouncyCastle.Crypto;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;
using System.Security.Cryptography;
using System.Text;

public static class RsaCryptoService
{
	public static byte[] Encrypt(byte[] data, RSAParameters publicKey, bool doOaepPadding)
	{
		using var rsa = new RSACryptoServiceProvider();
		rsa.ImportParameters(publicKey);
		return rsa.Encrypt(data, doOaepPadding);
	}

	public static byte[] Decrypt(byte[] data, RSAParameters privateKey, bool doOaepPadding)
	{
		using var rsa = new RSACryptoServiceProvider();
		rsa.ImportParameters(privateKey);
		return rsa.Decrypt(data, doOaepPadding);
	}
}

public static class RsaKeyConverter
{
	public static (string publicPem, string privatePem) GenerateKeyPair(int keySize = 1024)
	{
		using var rsa = new RSACryptoServiceProvider(keySize);
		return (ExportPublicKey(rsa), ExportPrivateKey(rsa));
	}

	public static RSAParameters ImportPrivateKey(string pem)
	{
		var pr      = new PemReader(new StringReader(pem));
		var keyPair = (AsymmetricCipherKeyPair)pr.ReadObject();
		var rsaParams = DotNetUtilities.ToRSAParameters((RsaPrivateCrtKeyParameters)keyPair.Private);
		using var csp = new RSACryptoServiceProvider();
		csp.ImportParameters(rsaParams);
		return csp.ExportParameters(true);
	}

	public static RSAParameters ImportPublicKey(string pem)
	{
		var pr        = new PemReader(new StringReader(pem));
		var publicKey = (AsymmetricKeyParameter)pr.ReadObject();
		var rsaParams = DotNetUtilities.ToRSAParameters((RsaKeyParameters)publicKey);
		using var csp = new RSACryptoServiceProvider();
		csp.ImportParameters(rsaParams);
		return csp.ExportParameters(false);
	}

	public static string ExportPrivateKey(RSACryptoServiceProvider csp)
	{
		if (csp.PublicOnly) throw new ArgumentException("CSP does not contain a private key");
		var p = csp.ExportParameters(true);
		using var stream = new MemoryStream();
		var writer = new BinaryWriter(stream);
		writer.Write((byte)0x30);
		using var inner = new MemoryStream();
		var iw = new BinaryWriter(inner);
		EncodeInteger(iw, new byte[] { 0x00 });
		EncodeInteger(iw, p.Modulus);  EncodeInteger(iw, p.Exponent);
		EncodeInteger(iw, p.D);        EncodeInteger(iw, p.P);
		EncodeInteger(iw, p.Q);        EncodeInteger(iw, p.DP);
		EncodeInteger(iw, p.DQ);       EncodeInteger(iw, p.InverseQ);
		EncodeLength(writer, (int)inner.Length);
		writer.Write(inner.GetBuffer(), 0, (int)inner.Length);
		return WrapPem("RSA PRIVATE KEY", stream.GetBuffer(), (int)stream.Length);
	}

	public static string ExportPublicKey(RSACryptoServiceProvider csp)
	{
		var p = csp.ExportParameters(false);
		using var stream = new MemoryStream();
		var writer = new BinaryWriter(stream);
		writer.Write((byte)0x30);
		using var inner = new MemoryStream();
		var iw = new BinaryWriter(inner);
		iw.Write((byte)0x30); EncodeLength(iw, 13);
		iw.Write((byte)0x06);
		var oid = new byte[] { 0x2a,0x86,0x48,0x86,0xf7,0x0d,0x01,0x01,0x01 };
		EncodeLength(iw, oid.Length); iw.Write(oid);
		iw.Write((byte)0x05); EncodeLength(iw, 0);
		iw.Write((byte)0x03);
		using var bits = new MemoryStream();
		var bw = new BinaryWriter(bits);
		bw.Write((byte)0x00); bw.Write((byte)0x30);
		using var param = new MemoryStream();
		var pw = new BinaryWriter(param);
		EncodeInteger(pw, p.Modulus); EncodeInteger(pw, p.Exponent);
		EncodeLength(bw, (int)param.Length); bw.Write(param.GetBuffer(), 0, (int)param.Length);
		EncodeLength(iw, (int)bits.Length); iw.Write(bits.GetBuffer(), 0, (int)bits.Length);
		EncodeLength(writer, (int)inner.Length); writer.Write(inner.GetBuffer(), 0, (int)inner.Length);
		return WrapPem("PUBLIC KEY", stream.GetBuffer(), (int)stream.Length);
	}

	private static string WrapPem(string label, byte[] buffer, int length)
	{
		var sb   = new StringBuilder();
		var b64  = Convert.ToBase64String(buffer, 0, length).ToCharArray();
		sb.Append($"-----BEGIN {label}-----\n");
		for (var i = 0; i < b64.Length; i += 64)
		{
			sb.Append(b64, i, Math.Min(64, b64.Length - i));
			sb.Append('\n');
		}
		sb.Append($"-----END {label}-----");
		return sb.ToString();
	}

	private static void EncodeLength(BinaryWriter w, int length)
	{
		if (length < 0x80) { w.Write((byte)length); return; }
		int bytesRequired = 0;
		for (var t = length; t > 0; t >>= 8) bytesRequired++;
		w.Write((byte)(bytesRequired | 0x80));
		for (var i = bytesRequired - 1; i >= 0; i--)
			w.Write((byte)(length >> (8 * i) & 0xff));
	}

	private static void EncodeInteger(BinaryWriter w, byte[] value)
	{
		w.Write((byte)0x02);
		int prefix = 0;
		while (prefix < value.Length && value[prefix] == 0) prefix++;
		if (value.Length - prefix == 0) { EncodeLength(w, 1); w.Write((byte)0); return; }
		bool needsZeroPad = value[prefix] > 0x7f;
		EncodeLength(w, value.Length - prefix + (needsZeroPad ? 1 : 0));
		if (needsZeroPad) w.Write((byte)0);
		for (var i = prefix; i < value.Length; i++) w.Write(value[i]);
	}
}
