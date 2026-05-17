using Microsoft.Extensions.Caching.Memory;

namespace Cache;

// Target: .NET 8+ (Server only)
public interface ILocalMemoryCache
{
	T GetOrCreateCache<T>(string key, Func<T> factory, MemoryCacheEntryOptions options = null) where T : class;
	T GetOrCreateCache<T1, T>(string key, Func<T1, T> factory, T1 p1, MemoryCacheEntryOptions options = null) where T : class;
	T GetOrCreateCache<T1, T2, T>(string key, Func<T1, T2, T> factory, T1 p1, T2 p2, MemoryCacheEntryOptions options = null) where T : class;

	delegate Task<T> FactoryAsync<T>();
	delegate Task<T> FactoryAsync<T1, T>(T1 p1);
	delegate Task<T> FactoryAsync<T1, T2, T>(T1 p1, T2 p2);

	Task<T> GetOrCreateCacheAsync<T>(string key, FactoryAsync<T> factory, MemoryCacheEntryOptions options = null) where T : class;
	Task<T> GetOrCreateCacheAsync<T1, T>(string key, FactoryAsync<T1, T> factory, T1 p1, MemoryCacheEntryOptions options = null) where T : class;
	Task<T> GetOrCreateCacheAsync<T1, T2, T>(string key, FactoryAsync<T1, T2, T> factory, T1 p1, T2 p2, MemoryCacheEntryOptions options = null) where T : class;

	T    GetFromCache<T>(string key) where T : class;
	void SetCache<T>(string key, T value, MemoryCacheEntryOptions options = null) where T : class;
	void ClearCache(string key);

	T    GetMessageFromCache<T>(string key) where T : class;
	void SetMessageCache<T>(string key, T value, MemoryCacheEntryOptions options = null) where T : class;

	T    GetJsonFromCache<T>(string key) where T : class;
	void SetJsonCache<T>(string key, T value, MemoryCacheEntryOptions options = null) where T : class;

	string GetStringFromCache(string key);
	void   SetStringCache(string key, string value, MemoryCacheEntryOptions options = null);
}
