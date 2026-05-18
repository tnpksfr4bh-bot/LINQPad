<Query Kind="Program">
  <NuGetReference Version="1.65.0.3297">Google.Apis.AndroidPublisher.v3</NuGetReference>
  <Namespace>Google.Apis.AndroidPublisher.v3</Namespace>
  <Namespace>Google.Apis.AndroidPublisher.v3.Data</Namespace>
  <Namespace>Google.Apis.Auth.OAuth2</Namespace>
  <Namespace>Google.Apis.Services</Namespace>
  <Namespace>System.Security.Cryptography.X509Certificates</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "GoogleIapService.cs"

// --- Test ---
// Set p12Path + serviceAccountId, then call Validate or GetSubscriptionStatus
async Task Main()
{
	string p12Path        = @"C:\Temp\service-account.p12";
	string serviceAccount = "your-account@appspot.gserviceaccount.com";

	var service = GoogleIapService.CreateService(p12Path, serviceAccount);

	// Validate subscription
	string bundleId = "com.example.app";
	string token    = "purchase-token-here";
	var result = await GoogleIapService.GetSubscriptionStatusAsync(service, bundleId, token);
	Console.WriteLine($"State : {result.SubscriptionState}");
	Console.WriteLine($"Order : {result.LatestOrderId}");
}
