<Query Kind="Program">
  <NuGetReference>K4os.Hash.xxHash</NuGetReference>
  <NuGetReference Version="10.0.1">System.IO.Hashing</NuGetReference>
  <Namespace>K4os.Hash.xxHash</Namespace>
  <Namespace>System.Buffers.Binary</Namespace>
  <Namespace>System.Globalization</Namespace>
  <Namespace>System.IO.Hashing</Namespace>
</Query>

#load "HashUtility.cs"

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
