using StackExchange.Redis;

namespace Cache;

// Target: .NET 8+ (Server only)
public static class RedisConnectionFactory
{
	public static IConnectionMultiplexer Create(RedisConfig config)
		=> ConnectionMultiplexer.Connect(config.GetConnectionString());

	public static IConnectionMultiplexer Create(string connectionString)
		=> ConnectionMultiplexer.Connect(connectionString);
}
