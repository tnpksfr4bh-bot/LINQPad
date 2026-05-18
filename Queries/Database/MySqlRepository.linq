<Query Kind="Program">
  <NuGetReference>Dapper</NuGetReference>
  <NuGetReference>Microsoft.EntityFrameworkCore</NuGetReference>
  <NuGetReference>Pomelo.EntityFrameworkCore.MySql</NuGetReference>
  <Namespace>Dapper</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore</Namespace>
  <Namespace>Microsoft.EntityFrameworkCore.Diagnostics</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>System.Data</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "MySqlRepository.cs"

// --- Test ---
async Task Main()
{
	var cs = "server=localhost;port=3306;database=test_db;uid=root;pwd=secret;AllowPublicKeyRetrieval=true;SslMode=none;";

	var provider = new ServiceCollection()
		.AddDbContext<AppDbContext>(o => o.UseMySql(cs, ServerVersion.AutoDetect(cs)))
		.AddScoped<IDbExtension<AppDbContext>, DbExtension<AppDbContext>>()
		.BuildServiceProvider();

	var repo = provider.GetRequiredService<IDbExtension<AppDbContext>>();

	// Raw SQL via Dapper
	var query = new DbQuery("SELECT 1 + 1 AS result");
	int val = await query.ScalarAsync<int>(cs);
	Console.WriteLine($"Scalar: {val}");
}
