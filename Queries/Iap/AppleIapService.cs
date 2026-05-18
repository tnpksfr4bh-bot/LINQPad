using Jose;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;

// Generates JWT tokens for Apple App Store Server API (in-app) and App Store Connect API
public static class AppleJwtTokenService
{
	// For App Store Server API — includes bundleId as "bid" claim (expires in 60 min)
	public static string CreateInAppToken(string bundleId, string issuerId, string keyId, string authKeyP8Base64)
	{
		var now = DateTime.UtcNow;
		var payload = new Dictionary<string, object>
		{
			{ "iat", ToUnixSeconds(now) },
			{ "exp", ToUnixSeconds(now.AddMinutes(60)) },
			{ "aud", "appstoreconnect-v1" },
			{ "iss", issuerId },
			{ "bid", bundleId },
		};
		return EncodeJwt(payload, keyId, authKeyP8Base64);
	}

	// For App Store Connect API — no bundleId, shorter expiry (15 min)
	public static string CreateAppStoreToken(string issuerId, string keyId, string authKeyP8Base64)
	{
		var now = DateTime.UtcNow;
		var payload = new Dictionary<string, object>
		{
			{ "exp", ToUnixSeconds(now.AddMinutes(15)) },
			{ "aud", "appstoreconnect-v1" },
			{ "iss", issuerId },
		};
		return EncodeJwt(payload, keyId, authKeyP8Base64);
	}

	private static string EncodeJwt(Dictionary<string, object> payload, string keyId, string authKeyP8Base64)
	{
		var header = new Dictionary<string, object> { { "kid", keyId } };
		using var ecdsa = ECDsa.Create();
		ecdsa.ImportPkcs8PrivateKey(Convert.FromBase64String(authKeyP8Base64), out _);
		return JWT.Encode(payload, ecdsa, JwsAlgorithm.ES256, header);
	}

	private static double ToUnixSeconds(DateTime dt)
		=> Math.Round((dt - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalSeconds, 0);
}

// Parses and validates StoreKit JWS transactions (x5c certificate chain in header)
public static class AppleReceiptValidator
{
	// Returns parsed payload as JToken, or null if validation fails
	public static JToken ParseTransactionJwt(string jwtString)
	{
		var handler  = new JwtSecurityTokenHandler();
		if (!handler.CanReadToken(jwtString))
			return null;

		var jwtToken = handler.ReadJwtToken(jwtString);
		var keys     = ExtractX5cPublicKeys(jwtToken.Header);

		try
		{
			handler.ValidateToken(jwtString, new TokenValidationParameters
			{
				ValidateIssuer    = false,
				ValidateAudience  = false,
				ValidateLifetime  = false,
				IssuerSigningKeys = keys,
			}, out var validated);

			return JToken.Parse(JsonConvert.SerializeObject(
				((JwtSecurityToken)validated).Payload));
		}
		catch (Exception ex)
		{
			Console.WriteLine($"JWT validation failed: {ex.Message}");
			return null;
		}
	}

	private static List<SecurityKey> ExtractX5cPublicKeys(JwtHeader header)
	{
		var keys = new List<SecurityKey>();
		if (!header.TryGetValue("x5c", out var x5cValue) || x5cValue is not IEnumerable<object> x5cList)
			return keys;

		foreach (var item in x5cList)
		{
			try
			{
				var cert  = new X509Certificate2(Encoding.UTF8.GetBytes(item.ToString()));
				var ecdsa = new ECDsaSecurityKey(cert.GetECDsaPublicKey());
				if (ecdsa != null) keys.Add(ecdsa);
			}
			catch { }
		}
		return keys;
	}
}
