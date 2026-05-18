<Query Kind="Program">
  <NuGetReference>jose-jwt</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Jose</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "AppleSubscriptionService.cs"

// --- Test ---
// Fill in credentials and run to fetch subscription status from Apple servers
async Task Main()
{
	string issuerId      = "";      // App Store Connect → Keys → Issuer ID
	string keyId         = "";      // Subscription Key ID
	string p8Base64      = "";      // .p8 file content, Base64-encoded (no headers)
	string bundleId      = "com.example.app";
	string transactionId = "2000000547841502";

	var service = new AppleSubscriptionService(bundleId, issuerId, keyId, p8Base64);
	var result  = await service.GetTransactionAsync(transactionId, sandbox: true);
	Console.WriteLine(result);
}
