<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "RetryHttpClient.cs"

// --- Test ---
async Task Main()
{
	using var client = new JsonHttpClient();

	// GET single object — expects { "status": "SUCCESS", ... }
	var obj = await client.GetSingleObjectAsync<JObject>(
		"https://httpbin.org/json",
		jsonNodePath: "",
		failure: (status, ex) => Console.WriteLine($"Error: {status} {ex?.Message}"));
	Console.WriteLine(obj);

	// GET collection from node path
	var list = await client.GetCollectionAsync<JObject>(
		"https://httpbin.org/json",
		jsonNodePath: "",
		failure: (status, ex) => Console.WriteLine($"Error: {status}"));
	Console.WriteLine($"Count: {list.Count()}");
}
