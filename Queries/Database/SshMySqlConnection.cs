using Microsoft.EntityFrameworkCore;
using Renci.SshNet;
using System.Net;

public class SshConfig
{
	public string Host       { get; set; }
	public int    Port       { get; set; } = 22;
	public string Username   { get; set; }
	public string PrivateKey { get; set; } // path to .pem / id_rsa
}

public class DbConfig
{
	public string Server   { get; set; }
	public int    Port     { get; set; } = 3306;
	public string Database { get; set; }
	public string UserId   { get; set; }
	public string Password { get; set; }

	public DbConfig Clone(int overridePort = 0)
		=> new()
		{
			Server   = Server,
			Port     = overridePort > 0 ? overridePort : Port,
			Database = Database,
			UserId   = UserId,
			Password = Password,
		};

	public string ToConnectionString()
		=> $"server={Server};port={Port};database={Database};uid={UserId};pwd={Password};AllowPublicKeyRetrieval=true;SslMode=none;";
}

public sealed class SshTunnel : IDisposable
{
	private readonly SshClient     _client;
	private readonly ForwardedPortLocal _port;

	internal SshTunnel(SshClient client, ForwardedPortLocal port)
	{
		_client = client;
		_port   = port;
	}

	public uint LocalPort => _port.BoundPort;

	public void Dispose()
	{
		_port.Stop();
		_client.Disconnect();
		_client.Dispose();
	}
}

public static class SshMySqlConnectionFactory
{
	// Opens SSH tunnel: localhost:{random} → dbConfig.Server:dbConfig.Port
	public static SshTunnel OpenTunnel(SshConfig ssh, DbConfig db)
	{
		var keyFile = new PrivateKeyFile(ssh.PrivateKey);
		var client  = new SshClient(ssh.Host, ssh.Port, ssh.Username, keyFile);
		client.Connect();

		var port = new ForwardedPortLocal("127.0.0.1", db.Server, (uint)db.Port);
		client.AddForwardedPort(port);
		port.Start();

		return new SshTunnel(client, port);
	}

	// Returns a DbContextOptions<T> that routes through the already-open tunnel
	public static DbContextOptions<T> BuildOptions<T>(DbConfig db) where T : DbContext
	{
		var cs = db.ToConnectionString();
		return new DbContextOptionsBuilder<T>()
			.UseMySql(cs, ServerVersion.AutoDetect(cs))
			.Options;
	}
}
