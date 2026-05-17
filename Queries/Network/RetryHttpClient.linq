<Query Kind="Program">
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>Newtonsoft.Json.Linq</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Net.Http</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

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

// ---

// HTTP client that parses JSON responses with optional JPath node selection.
// Expects { "status": "SUCCESS", ... } envelope; calls failure callback on any error.
public class JsonHttpClient : IDisposable
{
	private readonly HttpClient _client;

	public JsonHttpClient(HttpClient client = null)
		=> _client = client ?? new HttpClient();

	// Returns a single object; if jsonNodePath points to an array, returns the first element.
	public async Task<T> GetSingleObjectAsync<T>(
		string requestUri,
		string jsonNodePath = "",
		Action<string, Exception> failure = null)
	{
		HttpResponseMessage response = null;
		try
		{
			response = await _client.GetAsync(requestUri);
			if (!response.IsSuccessStatusCode)
			{
				failure?.Invoke(response.StatusCode.ToSnakeCaseString(), null);
				return default;
			}

			string content = await response.Content.ReadAsStringAsync();
			if (!IsValidJson(content)) { failure?.Invoke("INVALID_JSON", null); return default; }

			JToken json   = JToken.Parse(content);
			string status = json?.Value<string>("status");
			if (!string.IsNullOrEmpty(status) &&
			    !"SUCCESS".Equals(status, StringComparison.OrdinalIgnoreCase))
			{
				failure?.Invoke(status ?? "GENERAL_ERROR", null);
				return default;
			}

			if (string.IsNullOrEmpty(jsonNodePath))
				return JsonConvert.DeserializeObject<T>(content);

			JToken node = json?.SelectToken(jsonNodePath);
			if (node == null) { failure?.Invoke("NODE_NOT_FOUND", null); return default; }

			if (node.Type == JTokenType.Array)
			{
				var list = node.ToObject<List<T>>();
				return list?.Count > 0 ? list[0] : default;
			}
			return node.ToObject<T>();
		}
		catch (Exception ex) { failure?.Invoke("EXCEPTION", ex); return default; }
		finally { response?.Dispose(); }
	}

	// Returns a collection; wraps a non-array node in a single-element enumerable.
	public async Task<IEnumerable<T>> GetCollectionAsync<T>(
		string requestUri,
		string jsonNodePath = "",
		Action<string, Exception> failure = null)
	{
		HttpResponseMessage response = null;
		try
		{
			response = await _client.GetAsync(requestUri);
			if (!response.IsSuccessStatusCode)
			{
				failure?.Invoke(response.StatusCode.ToSnakeCaseString(), null);
				return Enumerable.Empty<T>();
			}

			string content = await response.Content.ReadAsStringAsync();
			if (!IsValidJson(content)) { failure?.Invoke("INVALID_JSON", null); return Enumerable.Empty<T>(); }

			JToken json   = JToken.Parse(content);
			string status = json?.Value<string>("status");
			if (!string.IsNullOrEmpty(status) &&
			    !"SUCCESS".Equals(status, StringComparison.OrdinalIgnoreCase))
			{
				failure?.Invoke(status ?? "GENERAL_ERROR", null);
				return Enumerable.Empty<T>();
			}

			if (string.IsNullOrEmpty(jsonNodePath))
				return JsonConvert.DeserializeObject<IEnumerable<T>>(content) ?? Enumerable.Empty<T>();

			JToken node = json?.SelectToken(jsonNodePath);
			if (node == null) { failure?.Invoke("NODE_NOT_FOUND", null); return Enumerable.Empty<T>(); }

			if (node.Type != JTokenType.Array)
			{
				var item = node.ToObject<T>();
				return item != null ? new[] { item } : Enumerable.Empty<T>();
			}
			return node.Children().Select(t => t.ToObject<T>());
		}
		catch (Exception ex) { failure?.Invoke("EXCEPTION", ex); return Enumerable.Empty<T>(); }
		finally { response?.Dispose(); }
	}

	// POST JSON body, returns deserialized response
	public async Task<T> PostJsonAsync<T>(
		string requestUri,
		object body,
		Action<string, Exception> failure = null)
	{
		HttpResponseMessage response = null;
		try
		{
			var json    = JsonConvert.SerializeObject(body);
			var content = new StringContent(json, System.Text.Encoding.UTF8, "application/json");
			response    = await _client.PostAsync(requestUri, content);
			if (!response.IsSuccessStatusCode)
			{
				failure?.Invoke(response.StatusCode.ToSnakeCaseString(), null);
				return default;
			}
			string result = await response.Content.ReadAsStringAsync();
			return JsonConvert.DeserializeObject<T>(result);
		}
		catch (Exception ex) { failure?.Invoke("EXCEPTION", ex); return default; }
		finally { response?.Dispose(); }
	}

	public static bool IsValidJson(string json)
	{
		try { return JToken.Parse(json) != null; } catch { return false; }
	}

	public void Dispose() => _client?.Dispose();
}

public static class HttpStatusCodeExtensions
{
	public static string ToSnakeCaseString(this HttpStatusCode code)
	{
		string name = code.ToString();
		return Regex.Replace(name, "(?<=[a-z])(?=[A-Z])", "_").ToUpper();
	}
}
