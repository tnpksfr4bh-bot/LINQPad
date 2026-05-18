<Query Kind="Program">
  <NuGetReference>AWSSDK.DynamoDBv2</NuGetReference>
  <Namespace>Amazon.DynamoDBv2</Namespace>
  <Namespace>Amazon.DynamoDBv2.Model</Namespace>
  <Namespace>Amazon.Runtime</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "DynamoDao.cs"

// --- Test ---
async Task Main()
{
	var creds  = new BasicAWSCredentials("ACCESS_KEY", "SECRET_KEY");
	var client = new AmazonDynamoDBClient(creds, Amazon.RegionEndpoint.APNortheast2);
	var dao    = new DynamoDao(client);

	string table    = "Users";
	string hashKey  = "UserId";
	string rangeKey = "CreatedAt";

	// Create table
	await dao.CreateTableStringHashAsync(table, hashKey, rangeKey);

	// Add item
	var item = new Dictionary<string, AttributeValue>
	{
		[hashKey]  = new AttributeValue { S = "user-001" },
		[rangeKey] = new AttributeValue { N = DateTimeOffset.UtcNow.ToUnixTimeSeconds().ToString() },
		["Name"]   = new AttributeValue { S = "Alice" },
	};
	await dao.AddItemAsync(table, item);

	// Get item
	var result = await dao.GetItemSingleAsync(table, hashKey, "user-001", rangeKey);
	Console.WriteLine($"Got {result.Count} attributes");
}
