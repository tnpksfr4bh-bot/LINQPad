using Microsoft.Extensions.Caching.Distributed;

namespace Cache;

// Target: .NET 8+ (Server only)
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
