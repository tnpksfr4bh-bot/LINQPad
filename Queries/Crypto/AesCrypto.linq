<Query Kind="Statements">
  <IncludeAspNet>true</IncludeAspNet>
  <Namespace>System.Security.Cryptography</Namespace>
</Query>

#load "AesCryptoService.cs"

// --- Test ---
var key      = "9gBo8r5rbzOHj0S5Xmv2biYwvXm1XmRc"; // 32 bytes
var iv       = "rdVsVxxOFQzODieV";                   // 16 bytes
var original = "Hello, AES!";

var encrypted = AesCryptoService.Encrypt(original, key, iv, useUrlSafe: true);
var decrypted = AesCryptoService.Decrypt(encrypted, key, iv);

Console.WriteLine($"Encrypted : {encrypted}");
Console.WriteLine($"Decrypted : {decrypted}");
Console.WriteLine($"SecureRandom(24): {AesCryptoService.SecureRandom(24)}");
