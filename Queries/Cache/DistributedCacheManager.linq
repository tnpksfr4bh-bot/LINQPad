<Query Kind="Program">
  <NuGetReference Version="2.3.85">MessagePack</NuGetReference>
  <NuGetReference>Microsoft.Extensions.Caching.StackExchangeRedis</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Cache</Namespace>
  <Namespace>Microsoft.Extensions.Caching.Distributed</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>StackExchange.Redis</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "../../Src/Cache/IRedisDistributedCache.cs"
#load "../../Src/Cache/RedisDistributedCache.cs"

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
