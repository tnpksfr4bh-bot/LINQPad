using System.Diagnostics;
using MessagePack;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace Cache;

// Target: .NET 8+ (Server only)
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
