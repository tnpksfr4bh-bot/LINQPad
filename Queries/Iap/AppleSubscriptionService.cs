using Jose;
using System.Net.Http;
using System.Security.Cryptography;

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
		return JWT.Encode(payload, ecdsa, JwsAlgorithm.ES256, header);
	}

	private static string Base(bool sandbox) => sandbox ? SandboxBase : ProdBase;

	private static double ToUnixSec(DateTime dt)
		=> Math.Round((dt - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds, 0);
}
