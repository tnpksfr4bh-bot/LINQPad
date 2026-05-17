using System.Text;

namespace Cache;

// Target: .NET 8+ (Server only)
// appsettings.json example:
// {
//   "RedisConfig": { "ServerIp": "redis.host", "Port": 6379, "Password": "secret", "Ssl": true }
//   "RedisProxyConfig": { "ServerIp": "127.0.0.1", "Port": 6380, "Ssl": false }  ← stunnel/SSH proxy
// }
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
