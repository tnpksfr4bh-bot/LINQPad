using MessagePack;
using Microsoft.Extensions.Caching.Memory;
using Newtonsoft.Json;

namespace Cache;

// Target: .NET 8+ (Server only)
// Default: SlidingExpiration 2h. Thread-safe via double-checked lock.
public class LocalMemoryCache : ILocalMemoryCache
{
	private static readonly object _lock = new();
	private readonly IMemoryCache _cache;

	public LocalMemoryCache(IMemoryCache cache) => _cache = cache;

	private MemoryCacheEntryOptions DefaultOptions()
		=> new() { SlidingExpiration = TimeSpan.FromHours(2) };

	public void SetCache<T>(string key, T value, MemoryCacheEntryOptions options = null) where T : class
		=> _cache.Set(key, value, options ?? DefaultOptions());

	public T GetFromCache<T>(string key) where T : class
		=> _cache.TryGetValue(key, out T v) ? v : null;

	public void ClearCache(string key) => _cache.Remove(key);

	public T GetOrCreateCache<T>(string key, Func<T> factory, MemoryCacheEntryOptions options = null) where T : class
	{
		if (_cache.TryGetValue(key, out T hit)) return hit;
		lock (_lock)
		{
			if (_cache.TryGetValue(key, out T hit2)) return hit2;
			var v = factory();
			_cache.Set(key, v, options ?? DefaultOptions());
			return v;
		}
	}

	public T GetOrCreateCache<T1, T>(string key, Func<T1, T> factory, T1 p1, MemoryCacheEntryOptions options = null) where T : class
	{
		if (_cache.TryGetValue(key, out T hit)) return hit;
		lock (_lock)
		{
			if (_cache.TryGetValue(key, out T hit2)) return hit2;
			var v = factory(p1);
			_cache.Set(key, v, options ?? DefaultOptions());
			return v;
		}
	}

	public T GetOrCreateCache<T1, T2, T>(string key, Func<T1, T2, T> factory, T1 p1, T2 p2, MemoryCacheEntryOptions options = null) where T : class
	{
		if (_cache.TryGetValue(key, out T hit)) return hit;
		lock (_lock)
		{
			if (_cache.TryGetValue(key, out T hit2)) return hit2;
			var v = factory(p1, p2);
			_cache.Set(key, v, options ?? DefaultOptions());
			return v;
		}
	}

	public async Task<T> GetOrCreateCacheAsync<T>(string key, ILocalMemoryCache.FactoryAsync<T> factory, MemoryCacheEntryOptions options = null) where T : class
	{
		if (_cache.TryGetValue(key, out T hit)) return hit;
		lock (_lock) { if (_cache.TryGetValue(key, out T h2)) return h2; }
		var v = await factory();
		lock (_lock) { _cache.Set(key, v, options ?? DefaultOptions()); }
		return v;
	}

	public async Task<T> GetOrCreateCacheAsync<T1, T>(string key, ILocalMemoryCache.FactoryAsync<T1, T> factory, T1 p1, MemoryCacheEntryOptions options = null) where T : class
	{
		if (_cache.TryGetValue(key, out T hit)) return hit;
		lock (_lock) { if (_cache.TryGetValue(key, out T h2)) return h2; }
		var v = await factory(p1);
		lock (_lock) { _cache.Set(key, v, options ?? DefaultOptions()); }
		return v;
	}

	public async Task<T> GetOrCreateCacheAsync<T1, T2, T>(string key, ILocalMemoryCache.FactoryAsync<T1, T2, T> factory, T1 p1, T2 p2, MemoryCacheEntryOptions options = null) where T : class
	{
		if (_cache.TryGetValue(key, out T hit)) return hit;
		lock (_lock) { if (_cache.TryGetValue(key, out T h2)) return h2; }
		var v = await factory(p1, p2);
		lock (_lock) { _cache.Set(key, v, options ?? DefaultOptions()); }
		return v;
	}

	public void SetMessageCache<T>(string key, T value, MemoryCacheEntryOptions options = null) where T : class
		=> _cache.Set(key, MessagePackSerializer.Serialize(value), options ?? DefaultOptions());

	public T GetMessageFromCache<T>(string key) where T : class
	{
		if (!_cache.TryGetValue(key, out byte[] bytes) || bytes == null) return null;
		return MessagePackSerializer.Deserialize<T>(bytes);
	}

	public void SetJsonCache<T>(string key, T value, MemoryCacheEntryOptions options = null) where T : class
		=> _cache.Set(key, JsonConvert.SerializeObject(value), options ?? DefaultOptions());

	public T GetJsonFromCache<T>(string key) where T : class
	{
		if (!_cache.TryGetValue(key, out string json) || json == null) return null;
		return JsonConvert.DeserializeObject<T>(json);
	}

	public string GetStringFromCache(string key)
		=> _cache.TryGetValue(key, out string v) ? v : null;

	public void SetStringCache(string key, string value, MemoryCacheEntryOptions options = null)
		=> _cache.Set(key, value, options ?? DefaultOptions());
}
