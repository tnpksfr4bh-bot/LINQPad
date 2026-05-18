<Query Kind="Statements">
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

#load "SecureStringGenerator.cs"

// --- Test ---
Console.WriteLine("--- Alphanumeric (length 16) ---");
for (int i = 0; i < 5; i++)
	Console.WriteLine(SecureStringGenerator.Generate(16));

Console.WriteLine("--- AppId (length 16 / 32) ---");
Console.WriteLine(SecureStringGenerator.GenerateAppId(16));
Console.WriteLine(SecureStringGenerator.GenerateAppId(32));
