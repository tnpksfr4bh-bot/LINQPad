<Query Kind="Program">
  <NuGetReference>K4os.Hash.xxHash</NuGetReference>
  <NuGetReference Version="10.0.1">System.IO.Hashing</NuGetReference>
  <Namespace>K4os.Hash.xxHash</Namespace>
  <Namespace>System.Buffers.Binary</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.IO.Hashing</Namespace>
</Query>

async Task Main()
{
	var text = "hello hash";
	var data = Encoding.UTF8.GetBytes(text);

	Console.WriteLine($"Input   : {text}");
	Console.WriteLine($"CRC32   : {CrcHasher.Crc32Hex(data)}");
	Console.WriteLine($"CRC64   : {CrcHasher.Crc64Hex(data)}");
	Console.WriteLine($"XXH64   : {XxHasher.Xxh64Hex(data)}");
	Console.WriteLine($"XXH3-64 : {XxHasher.Xxh3Hex(data)}");
}

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
