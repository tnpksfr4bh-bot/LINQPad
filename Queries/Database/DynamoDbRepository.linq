<Query Kind="Program">
  <NuGetReference>AWSSDK.DynamoDBv2</NuGetReference>
  <Namespace>Amazon.DynamoDBv2</Namespace>
  <Namespace>Amazon.DynamoDBv2.Model</Namespace>
  <Namespace>Amazon.Runtime</Namespace>
  <Namespace>System.Net</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

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

// ---

public class DynamoDao
{
	private readonly IAmazonDynamoDB _client;
	public DynamoDao(IAmazonDynamoDB client) => _client = client;

	// --- Read ---

	public async Task<Dictionary<string, AttributeValue>> GetItemSingleAsync(
		string table, string hashKeyName, string hashKeyValue, string rangeKeyName = null, string rangeKeyValue = null)
	{
		var key = new Dictionary<string, AttributeValue>
		{
			[hashKeyName] = new() { S = hashKeyValue }
		};
		if (rangeKeyName != null && rangeKeyValue != null)
			key[rangeKeyName] = new AttributeValue { S = rangeKeyValue };

		var response = await _client.GetItemAsync(new GetItemRequest { TableName = table, Key = key });
		return response.Item;
	}

	public async Task<List<Dictionary<string, AttributeValue>>> GetItemListNumberRangeAsync(
		string table, string hashKeyName, string hashKeyValue,
		string rangeKeyName, long from, long to, int limit = 100)
	{
		var request = new QueryRequest
		{
			TableName                 = table,
			KeyConditionExpression    = $"#{hashKeyName} = :hk AND #{rangeKeyName} BETWEEN :from AND :to",
			ExpressionAttributeNames  = new() { [$"#{hashKeyName}"] = hashKeyName, [$"#{rangeKeyName}"] = rangeKeyName },
			ExpressionAttributeValues = new()
			{
				[":hk"]   = new AttributeValue { S = hashKeyValue },
				[":from"] = new AttributeValue { N = from.ToString() },
				[":to"]   = new AttributeValue { N = to.ToString() },
			},
			Limit = limit,
		};
		var response = await _client.QueryAsync(request);
		return response.Items;
	}

	public async Task<List<Dictionary<string, AttributeValue>>> GetItemListNumberMatchAsync(
		string table, string hashKeyName, string hashKeyValue,
		string rangeKeyName, long rangeKeyValue, int limit = 100)
	{
		var request = new QueryRequest
		{
			TableName                 = table,
			KeyConditionExpression    = $"#{hashKeyName} = :hk AND #{rangeKeyName} = :rk",
			ExpressionAttributeNames  = new() { [$"#{hashKeyName}"] = hashKeyName, [$"#{rangeKeyName}"] = rangeKeyName },
			ExpressionAttributeValues = new()
			{
				[":hk"] = new AttributeValue { S = hashKeyValue },
				[":rk"] = new AttributeValue { N = rangeKeyValue.ToString() },
			},
			Limit = limit,
		};
		var response = await _client.QueryAsync(request);
		return response.Items;
	}

	public async Task<List<Dictionary<string, AttributeValue>>> GetItemListHashAsync(
		string table, string hashKeyName, string hashKeyValue, int limit = 100)
	{
		var request = new QueryRequest
		{
			TableName                 = table,
			KeyConditionExpression    = $"#{hashKeyName} = :hk",
			ExpressionAttributeNames  = new() { [$"#{hashKeyName}"] = hashKeyName },
			ExpressionAttributeValues = new() { [":hk"] = new AttributeValue { S = hashKeyValue } },
			Limit                     = limit,
		};
		var response = await _client.QueryAsync(request);
		return response.Items;
	}

	// Returns most recent N items (ScanIndexForward = false)
	public async Task<List<Dictionary<string, AttributeValue>>> GetItemListLatestAsync(
		string table, string hashKeyName, string hashKeyValue,
		string rangeKeyName, int limit = 10)
	{
		var request = new QueryRequest
		{
			TableName                 = table,
			KeyConditionExpression    = $"#{hashKeyName} = :hk",
			ExpressionAttributeNames  = new() { [$"#{hashKeyName}"] = hashKeyName },
			ExpressionAttributeValues = new() { [":hk"] = new AttributeValue { S = hashKeyValue } },
			ScanIndexForward          = false,
			Limit                     = limit,
		};
		var response = await _client.QueryAsync(request);
		return response.Items;
	}

	// --- Write ---

	// Fails if item already exists (add-only)
	public async Task<bool> AddItemAsync(string table, Dictionary<string, AttributeValue> item)
	{
		try
		{
			await _client.PutItemAsync(new PutItemRequest
			{
				TableName           = table,
				Item                = item,
				ConditionExpression = "attribute_not_exists(#pk)",
				ExpressionAttributeNames = new() { ["#pk"] = item.Keys.First() },
			});
			return true;
		}
		catch (ConditionalCheckFailedException) { return false; }
	}

	// Upsert — overwrites if exists
	public async Task PutItemAsync(string table, Dictionary<string, AttributeValue> item)
		=> await _client.PutItemAsync(new PutItemRequest { TableName = table, Item = item });

	public async Task UpdateItemAsync(
		string table,
		Dictionary<string, AttributeValue> key,
		string updateExpression,
		Dictionary<string, AttributeValue> values,
		Dictionary<string, string> names = null)
	{
		await _client.UpdateItemAsync(new UpdateItemRequest
		{
			TableName                 = table,
			Key                       = key,
			UpdateExpression          = updateExpression,
			ExpressionAttributeValues = values,
			ExpressionAttributeNames  = names,
		});
	}

	public async Task DeleteItemAsync(string table, Dictionary<string, AttributeValue> key)
		=> await _client.DeleteItemAsync(new DeleteItemRequest { TableName = table, Key = key });

	// --- DDL ---

	public async Task CreateTableStringHashAsync(string table, string hashKeyName, string rangeKeyName = null,
		long readCapacity = 5, long writeCapacity = 5)
	{
		var keys   = new List<KeySchemaElement> { new(hashKeyName, KeyType.HASH) };
		var attrs  = new List<AttributeDefinition> { new(hashKeyName, ScalarAttributeType.S) };

		if (rangeKeyName != null)
		{
			keys.Add(new KeySchemaElement(rangeKeyName, KeyType.RANGE));
			attrs.Add(new AttributeDefinition(rangeKeyName, ScalarAttributeType.N));
		}

		await _client.CreateTableAsync(new CreateTableRequest
		{
			TableName            = table,
			KeySchema            = keys,
			AttributeDefinitions = attrs,
			ProvisionedThroughput = new ProvisionedThroughput(readCapacity, writeCapacity),
		});
	}

	public async Task CreateIndexWithStringAsync(string table, string indexName,
		string hashKeyName, string rangeKeyName = null, long read = 5, long write = 5)
	{
		var keys  = new List<KeySchemaElement> { new(hashKeyName, KeyType.HASH) };
		var attrs = new List<AttributeDefinition> { new(hashKeyName, ScalarAttributeType.S) };
		if (rangeKeyName != null)
		{
			keys.Add(new KeySchemaElement(rangeKeyName, KeyType.RANGE));
			attrs.Add(new AttributeDefinition(rangeKeyName, ScalarAttributeType.S));
		}

		await _client.UpdateTableAsync(new UpdateTableRequest
		{
			TableName            = table,
			AttributeDefinitions = attrs,
			GlobalSecondaryIndexUpdates = new()
			{
				new GlobalSecondaryIndexUpdate
				{
					Create = new CreateGlobalSecondaryIndexAction
					{
						IndexName             = indexName,
						KeySchema             = keys,
						Projection            = new Projection { ProjectionType = ProjectionType.ALL },
						ProvisionedThroughput = new ProvisionedThroughput(read, write),
					}
				}
			}
		});
	}

	public async Task CreateIndexWithLongAsync(string table, string indexName,
		string hashKeyName, string rangeKeyName = null, long read = 5, long write = 5)
	{
		var keys  = new List<KeySchemaElement> { new(hashKeyName, KeyType.HASH) };
		var attrs = new List<AttributeDefinition> { new(hashKeyName, ScalarAttributeType.S) };
		if (rangeKeyName != null)
		{
			keys.Add(new KeySchemaElement(rangeKeyName, KeyType.RANGE));
			attrs.Add(new AttributeDefinition(rangeKeyName, ScalarAttributeType.N));
		}

		await _client.UpdateTableAsync(new UpdateTableRequest
		{
			TableName            = table,
			AttributeDefinitions = attrs,
			GlobalSecondaryIndexUpdates = new()
			{
				new GlobalSecondaryIndexUpdate
				{
					Create = new CreateGlobalSecondaryIndexAction
					{
						IndexName             = indexName,
						KeySchema             = keys,
						Projection            = new Projection { ProjectionType = ProjectionType.ALL },
						ProvisionedThroughput = new ProvisionedThroughput(read, write),
					}
				}
			}
		});
	}

	public async Task<DescribeTableResponse> DescribeTableAsync(string table)
		=> await _client.DescribeTableAsync(table);

	// Maps AWS error codes to HTTP status equivalents
	public static int KnownExceptionParseStatusResult(Exception ex)
	{
		if (ex is AmazonDynamoDBException dex)
		{
			return dex.ErrorCode switch
			{
				"ConditionalCheckFailedException" => 409,
				"ResourceNotFoundException"       => 404,
				"ProvisionedThroughputExceededException" => 429,
				"RequestLimitExceeded"            => 429,
				"ItemCollectionSizeLimitExceededException" => 413,
				_ => (int)dex.StatusCode,
			};
		}
		return 500;
	}
}
