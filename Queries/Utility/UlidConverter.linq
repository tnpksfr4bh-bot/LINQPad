<Query Kind="Program">
  <NuGetReference>ulid.net</NuGetReference>
  <Namespace>CSharp.Ulid</Namespace>
  <Namespace>System.Globalization</Namespace>
</Query>

#load "UlidConverter.cs"

// --- Test ---
void Main()
{
	// New random ULID
	string newUlid = UlidConverter.New();
	Console.WriteLine($"New ULID   : {newUlid}");

	// Encode a long into the randomness section at a specific timestamp
	long   longId  = 5290776819530245L;
	string dateStr = "2020-02-14T11:37:30";
	string encoded = UlidConverter.FromLong(longId, dateStr);
	Console.WriteLine($"Encoded    : {encoded}");

	// Decode back
	long decoded = UlidConverter.ToLong(encoded);
	Console.WriteLine($"Decoded ID : {decoded}");
	Console.WriteLine($"Match      : {longId == decoded}");

	// Extract timestamp
	DateTime ts = UlidConverter.GetTimestamp(encoded);
	Console.WriteLine($"Timestamp  : {ts:s}");
}
