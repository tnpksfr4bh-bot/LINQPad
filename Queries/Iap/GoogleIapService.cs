using Google.Apis.AndroidPublisher.v3;
using Google.Apis.AndroidPublisher.v3.Data;
using Google.Apis.Auth.OAuth2;
using Google.Apis.Services;
using System.Security.Cryptography.X509Certificates;

public static class GoogleIapService
{
	public static AndroidPublisherService CreateService(string p12Path, string serviceAccountId)
	{
		var certBytes   = File.ReadAllBytes(p12Path);
		var certificate = new X509Certificate2(certBytes, "notasecret", X509KeyStorageFlags.MachineKeySet);
		var credentials = new ServiceAccountCredential(
			new ServiceAccountCredential.Initializer(serviceAccountId)
			{
				Scopes = new[] { AndroidPublisherService.Scope.Androidpublisher }
			}.FromCertificate(certificate));

		return new AndroidPublisherService(new BaseClientService.Initializer
		{
			HttpClientInitializer = credentials
		});
	}

	// Validate consumable product purchase — returns PurchaseState (0 = purchased)
	public static async Task<ProductPurchase> GetProductPurchaseAsync(
		AndroidPublisherService service, string bundleId, string productId, string token)
	{
		var request = service.Purchases.Products.Get(bundleId, productId, token);
		return await request.ExecuteAsync();
	}

	// Validate subscription purchase via v2 API
	public static async Task<SubscriptionPurchaseV2> GetSubscriptionStatusAsync(
		AndroidPublisherService service, string bundleId, string token)
	{
		var request = service.Purchases.Subscriptionsv2.Get(bundleId, token);
		return await request.ExecuteAsync();
	}

	// Returns true when subscription is active and paid
	public static bool IsSubscriptionActive(SubscriptionPurchaseV2 purchase)
		=> purchase.SubscriptionState == "SUBSCRIPTION_STATE_ACTIVE";

	// Returns expiry as UTC DateTime for the first line item
	public static DateTime? GetExpiryTime(SubscriptionPurchaseV2 purchase)
	{
		var item = purchase.LineItems?.FirstOrDefault();
		if (item == null) return null;
		return DateTime.TryParse(item.ExpiryTimeRaw, out var dt) ? dt : null;
	}
}

// Parsed result from GoogleIapService.GetProductPurchaseAsync
public class GoogleProductPurchaseResult
{
	public GoogleProductPurchaseResult(ProductPurchase result)
	{
		long ms      = result.PurchaseTimeMillis ?? 0;
		PurchaseTime = DateTimeOffset.FromUnixTimeMilliseconds(ms).UtcDateTime;
		RegionCode              = result.RegionCode;
		ConsumptionState        = result.ConsumptionState ?? 0;
		AcknowledgementState    = result.AcknowledgementState ?? 0;
		Kind                    = result?.Kind?.Split("#")[1] ?? string.Empty;
		OrderId                 = result?.OrderId ?? string.Empty;
	}

	public string   OrderId              { get; }
	public string   Kind                 { get; }
	public int      AcknowledgementState { get; }
	public int      ConsumptionState     { get; }
	public DateTime PurchaseTime         { get; }
	public string   RegionCode           { get; }
}
