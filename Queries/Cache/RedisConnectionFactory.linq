<Query Kind="Program">
  <NuGetReference>Microsoft.Extensions.Caching.StackExchangeRedis</NuGetReference>
  <Namespace>StackExchange.Redis</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

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

// ---

public class RedisConfig
{
	public string ServerIp { get; set; }
	public int    Port     { get; set; } = 6379;
	public string Password { get; set; }
	public bool   Ssl      { get; set; }

	// Format: IP:PORT[,password=PWD][,ssl=true]
	public string GetConnectionString()
	{
		var sb = new StringBuilder($"{ServerIp}:{Port}");
		if (!string.IsNullOrEmpty(Password)) sb.Append($",password={Password}");
		if (Ssl)                             sb.Append(",ssl=true");
		return sb.ToString();
	}
}

// appsettings.json example:
// {
//   "RedisConfig": { "ServerIp": "redis.host", "Port": 6379, "Password": "secret", "Ssl": true }
//   "RedisProxyConfig": { "ServerIp": "127.0.0.1", "Port": 6380, "Ssl": false }  ← stunnel/SSH proxy
// }
public static class RedisConnectionFactory
{
	public static IConnectionMultiplexer Create(RedisConfig config)
		=> ConnectionMultiplexer.Connect(config.GetConnectionString());

	public static IConnectionMultiplexer Create(string connectionString)
		=> ConnectionMultiplexer.Connect(connectionString);
}
