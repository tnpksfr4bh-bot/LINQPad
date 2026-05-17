<Query Kind="Program">
  <NuGetReference Version="2.3.85">MessagePack</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Caching.StackExchangeRedis</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>MessagePack</Namespace>
  <Namespace>Microsoft.Extensions.Caching.Distributed</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>StackExchange.Redis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

// --- Test ---
async Task Main()
{
	// IDistributedCache (Redis) 기반 분산 캐시
	var provider = new ServiceCollection()
		.AddStackExchangeRedisCache(o =>
		{
			o.Configuration = "localhost:6379";
			o.InstanceName  = "test:";
		})
		.AddScoped<IRedisDistributedCache, RedisDistributedCache>()
		.BuildServiceProvider();

	var cache = provider.GetRequiredService<IRedisDistributedCache>();

	await cache.SetStringCache("hello", "world");
	var val = await cache.GetStringFromCache("hello");
	Console.WriteLine($"Got: {val}");

	await cache.ClearCache("hello");
}

// ---

public interface IRedisDistributedCache
{
	Task<T>    GetMessageFromCache<T>(string key) where T : class;
	Task       SetMessageCache<T>(string key, T value, DistributedCacheEntryOptions options = null) where T : class;
	Task<T>    GetOrCreateMessageCache<T>(string key, Func<Task<T>> factory, DistributedCacheEntryOptions options = null) where T : class;

	Task<T>    GetJsonFromCache<T>(string key) where T : class;
	Task       SetJsonCache<T>(string key, T value, DistributedCacheEntryOptions options = null) where T : class;
	Task<T>    GetOrCreateJsonCache<T>(string key, Func<Task<T>> factory, DistributedCacheEntryOptions options = null) where T : class;

	Task<string> GetStringFromCache(string key);
	Task         SetStringCache(string key, string value, DistributedCacheEntryOptions options = null);
	Task<string> GetOrCreateStringCache(string key, Func<Task<string>> factory, DistributedCacheEntryOptions options = null);

	Task ClearCache(string key);
}

// Default: SlidingExpiration 6h. Falls back to factory on RedisException.
public class RedisDistributedCache : IRedisDistributedCache
{
	private readonly IDistributedCache _cache;
	public RedisDistributedCache(IDistributedCache cache) => _cache = cache;

	// Default 6h — matches Lambda max execution time
	private DistributedCacheEntryOptions DefaultOptions()
		=> new() { SlidingExpiration = TimeSpan.FromHours(6) };

	public async Task SetMessageCache<T>(string key, T value, DistributedCacheEntryOptions options = null) where T : class
	{
		try
		{
			await _cache.SetAsync(key, MessagePackSerializer.Serialize(value), options ?? DefaultOptions());
		}
		catch (MessagePackSerializationException ex) { Debug.WriteLine(ex.Message); }
	}

	public async Task<T> GetMessageFromCache<T>(string key) where T : class
	{
		var bytes = await _cache.GetAsync(key);
		return bytes == null ? null : MessagePackSerializer.Deserialize<T>(bytes);
	}

	public async Task<T> GetOrCreateMessageCache<T>(string key, Func<Task<T>> factory, DistributedCacheEntryOptions options = null) where T : class
	{
		try
		{
			var bytes = await _cache.GetAsync(key);
			if (bytes != null)
				try { return MessagePackSerializer.Deserialize<T>(bytes); } catch { }

			var obj = await factory();
			try { await _cache.SetAsync(key, MessagePackSerializer.Serialize(obj), options ?? DefaultOptions()); } catch { }
			return obj;
		}
		catch (RedisException) { return await factory(); }
	}

	public async Task<T> GetJsonFromCache<T>(string key) where T : class
	{
		var s = await _cache.GetStringAsync(key);
		return s == null ? null : JsonConvert.DeserializeObject<T>(s);
	}

	public async Task SetJsonCache<T>(string key, T value, DistributedCacheEntryOptions options = null) where T : class
	{
		try { await _cache.SetStringAsync(key, JsonConvert.SerializeObject(value), options ?? DefaultOptions()); }
		catch (RedisException ex) { Debug.WriteLine(ex.Message); }
	}

	public async Task<T> GetOrCreateJsonCache<T>(string key, Func<Task<T>> factory, DistributedCacheEntryOptions options = null) where T : class
	{
		try
		{
			var s = await _cache.GetStringAsync(key);
			if (s != null) return JsonConvert.DeserializeObject<T>(s);
			var obj = await factory();
			await _cache.SetStringAsync(key, JsonConvert.SerializeObject(obj), options ?? DefaultOptions());
			return obj;
		}
		catch (RedisException) { return await factory(); }
	}

	public async Task<string> GetStringFromCache(string key)
	{
		try { return await _cache.GetStringAsync(key); }
		catch (RedisException ex) { Debug.WriteLine(ex.Message); return null; }
	}

	public async Task SetStringCache(string key, string value, DistributedCacheEntryOptions options = null)
	{
		try { await _cache.SetStringAsync(key, value, options ?? DefaultOptions()); }
		catch (RedisException ex) { Debug.WriteLine(ex.Message); }
	}

	public async Task<string> GetOrCreateStringCache(string key, Func<Task<string>> factory, DistributedCacheEntryOptions options = null)
	{
		try
		{
			var s = await _cache.GetStringAsync(key);
			if (s != null) return s;
			s = await factory();
			await _cache.SetStringAsync(key, s, options ?? DefaultOptions());
			return s;
		}
		catch (RedisException) { return await factory(); }
	}

	public async Task ClearCache(string key)
	{
		try { await _cache.RemoveAsync(key); }
		catch (RedisException ex) { Debug.WriteLine(ex.Message); }
	}
}
