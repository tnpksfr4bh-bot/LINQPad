<Query Kind="Program">
  <NuGetReference>AWSSDK.RDSDataService</NuGetReference>
  <Namespace>Amazon.RDSDataService</Namespace>
  <Namespace>Amazon.RDSDataService.Model</Namespace>
  <Namespace>Amazon.Runtime</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "AuroraDataService.cs"

// --- Test ---
async Task Main()
{
	var config = new DataServiceConfig
	{
		AccessKey  = "ACCESS_KEY",
		SecretKey  = "SECRET_KEY",
		Region     = "ap-northeast-2",
		SecretArn  = "arn:aws:secretsmanager:...",
		AuroraArn  = "arn:aws:rds:...",
		Database   = "my_db",
	};

	var svc = new AuroraDataService(config);

	var rows = await svc.ExecuteQueryAsync(
		"SELECT :p1 + :p2 AS result",
		new SqlParameter { Name = "p1", Value = new Field { LongValue = 3 } },
		new SqlParameter { Name = "p2", Value = new Field { LongValue = 4 } }
	);

	foreach (var row in rows)
		Console.WriteLine(row[0].LongValue); // 7
}
