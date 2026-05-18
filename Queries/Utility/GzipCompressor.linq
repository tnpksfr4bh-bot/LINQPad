<Query Kind="Program">
  <Namespace>System.IO.Compression</Namespace>
</Query>

#load "GzipCompressor.cs"

// --- Test ---
void Main()
{
	string original     = "Hello, World! 안녕하세요";
	string compressed   = GzipCompressor.Compress(original);
	string decompressed = GzipCompressor.Decompress(compressed);

	Console.WriteLine($"Original   : {original}");
	Console.WriteLine($"Compressed : {compressed}");
	Console.WriteLine($"Round-trip : {decompressed}");
	Console.WriteLine($"Match      : {original == decompressed}");
}
