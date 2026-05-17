<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.Caching.StackExchangeRedis</NuGetReference>
  <Namespace>Cache</Namespace>
  <Namespace>StackExchange.Redis</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "../../Src/Cache/RedisConfig.cs"
#load "../../Src/Cache/RedisConnectionFactory.cs"

// --- Test ---
void Main()
{
	var config = new RedisConfig
	{
		ServerIp = "localhost",
		Port     = 6379,
		Password = "",
		Ssl      = false,
	};

	Console.WriteLine($"ConnectionString: {config.GetConnectionString()}");

	var multiplexer = RedisConnectionFactory.Create(config);
	Console.WriteLine($"Connected: {multiplexer.IsConnected}");

	var db = multiplexer.GetDatabase(0);
	db.StringSet("ping", "pong", TimeSpan.FromSeconds(10));
	Console.WriteLine($"ping → {db.StringGet("ping")}");
}
