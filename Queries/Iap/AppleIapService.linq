<Query Kind="Program">
  <NuGetReference>jose-jwt</NuGetReference>
  <NuGetReference>System.IdentityModel.Tokens.Jwt</NuGetReference>
  <Namespace>Jose</Namespace>
  <Namespace>Microsoft.IdentityModel.Tokens</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.IdentityModel.Tokens.Jwt</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Security.Cryptography</Namespace>
  <Namespace>System.Security.Cryptography.X509Certificates</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "AppleIapService.cs"

// --- Test ---
// AppleJwtTokenService.CreateInAppToken(bundleId, issuerId, keyId, p8Base64);
// AppleJwtTokenService.CreateAppStoreToken(issuerId, keyId, p8Base64);
// AppleReceiptValidator.ParseTransactionJwt(jwtString);

async Task Main()
{
	// Replace with real values to test
	string sampleJwt = "eyJra..."; // paste a StoreKit JWS transaction here
	var payload = AppleReceiptValidator.ParseTransactionJwt(sampleJwt);
	Console.WriteLine(payload?.ToString(Formatting.Indented));
}
