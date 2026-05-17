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

// --- Test ---
// Fill in credentials and run to fetch subscription status from Apple servers
async Task Main()
{
	string issuerId     = "";      // App Store Connect → Keys → Issuer ID
	string keyId        = "";      // Subscription Key ID
	string p8Base64     = "";      // .p8 file content, Base64-encoded (no headers)
	string bundleId     = "com.example.app";
	string transactionId = "2000000547841502";

	var service = new AppleSubscriptionService(bundleId, issuerId, keyId, p8Base64);
	var result  = await service.GetTransactionAsync(transactionId, sandbox: true);
	Console.WriteLine(result);
}

// ---

public class AppleSubscriptionService
{
	private const string ProdBase    = "https://api.storekit.itunes.apple.com";
	private const string SandboxBase = "https://api.storekit-sandbox.itunes.apple.com";

	private readonly string _bundleId;
	private readonly string _issuerId;
	private readonly string _keyId;
	private readonly string _p8Base64;

	public AppleSubscriptionService(string bundleId, string issuerId, string keyId, string p8Base64)
	{
		_bundleId = bundleId;
		_issuerId = issuerId;
		_keyId    = keyId;
		_p8Base64 = p8Base64;
	}

	public Task<string> GetTransactionAsync(string transactionId, bool sandbox = false)
	{
		var url = $"{Base(sandbox)}/inApps/v1/transactions/{transactionId}";
		return FetchAsync(url);
	}

	public Task<string> GetAllSubscriptionStatusAsync(string transactionId, bool sandbox = false)
	{
		var url = $"{Base(sandbox)}/inApps/v1/subscriptions/{transactionId}";
		return FetchAsync(url);
	}

	private async Task<string> FetchAsync(string url)
	{
		var token = CreateInAppToken();
		using var client = new HttpClient();
		client.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
		client.DefaultRequestHeaders.Add("Accept", "application/json");
		var response = await client.GetAsync(url);
		return response.IsSuccessStatusCode
			? await response.Content.ReadAsStringAsync()
			: $"Error: {response.StatusCode}";
	}

	private string CreateInAppToken()
	{
		var now     = DateTime.UtcNow;
		var payload = new Dictionary<string, object>
		{
			{ "iat", ToUnixSec(now) },
			{ "exp", ToUnixSec(now.AddMinutes(60)) },
			{ "aud", "appstoreconnect-v1" },
			{ "iss", _issuerId },
			{ "bid", _bundleId },
		};
		var header = new Dictionary<string, object> { { "kid", _keyId } };
		using var ecdsa = ECDsa.Create();
		ecdsa.ImportPkcs8PrivateKey(Convert.FromBase64String(_p8Base64), out _);
		return Jose.JWT.Encode(payload, ecdsa, Jose.JwsAlgorithm.ES256, header);
	}

	private static string Base(bool sandbox) => sandbox ? SandboxBase : ProdBase;

	private static double ToUnixSec(DateTime dt)
		=> Math.Round((dt - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds, 0);
}
