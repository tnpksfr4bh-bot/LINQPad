<Query Kind="Program">
  <NuGetReference>Pomelo.EntityFrameworkCore.MySql</NuGetReference>
  <NuGetReference>SSH.NET</NuGetReference>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Renci.SshNet</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "SshMySqlConnection.cs"

// --- Test ---
void Main()
{
	var ssh = new SshConfig
	{
		Host       = "bastion.example.com",
		Port       = 22,
		Username   = "ubuntu",
		PrivateKey = @"C:\Keys\id_rsa",
	};

	var db = new DbConfig
	{
		Server   = "127.0.0.1",
		Port     = 3307,
		Database = "my_db",
		UserId   = "root",
		Password = "secret",
	};

	using var tunnel = SshMySqlConnectionFactory.OpenTunnel(ssh, db);
	Console.WriteLine($"Tunnel local port : {tunnel.LocalPort}");
	Console.WriteLine($"Connection string : {db.Clone(tunnel.LocalPort).ToConnectionString()}");
}
