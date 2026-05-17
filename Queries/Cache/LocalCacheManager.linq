<Query Kind="Program">
  <NuGetReference Version="2.3.85">MessagePack</NuGetReference>
  <NuGetReference>Newtonsoft.Json</NuGetReference>
  <Namespace>Cache</Namespace>
  <Namespace>Microsoft.Extensions.Caching.Memory</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Newtonsoft.Json</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "../../Src/Cache/ILocalMemoryCache.cs"
#load "../../Src/Cache/LocalMemoryCache.cs"

// --- Test ---
void Main()
{
	var provider = new ServiceCollection()
		.AddMemoryCache()
		.AddSingleton<ILocalMemoryCache, LocalMemoryCache>()
		.BuildServiceProvider();

	var cache = provider.GetRequiredService<ILocalMemoryCache>();

	var result = cache.GetOrCreateCache("test-key", () => new { Value = "hello", Time = DateTime.UtcNow });
	Console.WriteLine($"First  : {result.Value} @ {result.Time}");

	var result2 = cache.GetOrCreateCache("test-key", () => new { Value = "world", Time = DateTime.UtcNow });
	Console.WriteLine($"Cached : {result2.Value} @ {result2.Time}"); // still "hello"
}
