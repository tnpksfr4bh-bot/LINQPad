using K4os.Hash.xxHash;
using System.Globalization;
using System.IO.Hashing;

public static class CrcHasher
{
	public static string Crc32Hex(byte[] data)
		=> "0x" + Convert.ToHexString(Crc32.Hash(data).Reverse().ToArray()).ToLower();

	public static string Crc64Hex(byte[] data)
		=> Convert.ToHexString(Crc64.Hash(data)).ToLower();
}

public static class XxHasher
{
	public static string Xxh64Hex(byte[] data)
		=> XXH64.DigestOf(data).ToString("x16");

	public static string Xxh3Hex(byte[] data, ulong seed = 0)
		=> XxHash3.HashToUInt64(data, (long)seed).ToString("x16", CultureInfo.InvariantCulture);
}
