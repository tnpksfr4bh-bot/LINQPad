using Amazon.RDSDataService;
using Amazon.RDSDataService.Model;
using Amazon.Runtime;

public class DataServiceConfig
{
	public string AccessKey { get; set; }
	public string SecretKey { get; set; }
	public string Region    { get; set; }
	public string SecretArn { get; set; }
	public string AuroraArn { get; set; }
	public string Database  { get; set; }

	public AmazonRDSDataServiceClient CreateClient()
	{
		var creds  = new BasicAWSCredentials(AccessKey, SecretKey);
		var region = Amazon.RegionEndpoint.GetBySystemName(Region);
		return new AmazonRDSDataServiceClient(creds, region);
	}
}

public class AuroraDataService
{
	private readonly DataServiceConfig _config;

	public AuroraDataService(DataServiceConfig config) => _config = config;

	// Returns rows as List<List<Field>>
	public async Task<List<List<Field>>> ExecuteQueryAsync(string sql, params SqlParameter[] parameters)
	{
		using var client  = _config.CreateClient();
		var request = new ExecuteStatementRequest
		{
			ResourceArn = _config.AuroraArn,
			SecretArn   = _config.SecretArn,
			Database    = _config.Database,
			Sql         = sql,
			Parameters  = parameters.ToList(),
		};

		var response = await client.ExecuteStatementAsync(request);
		return response.Records;
	}

	// Returns rows mapped via selector
	public async Task<List<T>> ExecuteQueryAsync<T>(string sql, Func<List<Field>, T> selector, params SqlParameter[] parameters)
	{
		var records = await ExecuteQueryAsync(sql, parameters);
		return records.Select(selector).ToList();
	}

	// Fire-and-forget DML (INSERT / UPDATE / DELETE) — returns number of affected rows
	public async Task<long> ExecuteAsync(string sql, params SqlParameter[] parameters)
	{
		using var client  = _config.CreateClient();
		var request = new ExecuteStatementRequest
		{
			ResourceArn      = _config.AuroraArn,
			SecretArn        = _config.SecretArn,
			Database         = _config.Database,
			Sql              = sql,
			Parameters       = parameters.ToList(),
		};

		var response = await client.ExecuteStatementAsync(request);
		return response.NumberOfRecordsUpdated;
	}

	// Runs multiple DML statements in a single transaction (batch execute)
	public async Task<List<UpdateResult>> BatchExecuteAsync(string sql, List<List<SqlParameter>> paramSets)
	{
		using var client  = _config.CreateClient();
		var request = new BatchExecuteStatementRequest
		{
			ResourceArn = _config.AuroraArn,
			SecretArn   = _config.SecretArn,
			Database    = _config.Database,
			Sql         = sql,
			ParameterSets = paramSets,
		};

		var response = await client.BatchExecuteStatementAsync(request);
		return response.UpdateResults;
	}

	// Builds a typed SqlParameter (helper for common types)
	public static SqlParameter Param(string name, string value)
		=> new() { Name = name, Value = new Field { StringValue = value } };

	public static SqlParameter Param(string name, long value)
		=> new() { Name = name, Value = new Field { LongValue = value } };

	public static SqlParameter Param(string name, double value)
		=> new() { Name = name, Value = new Field { DoubleValue = value } };

	public static SqlParameter Param(string name, bool value)
		=> new() { Name = name, Value = new Field { BooleanValue = value } };

	public static SqlParameter ParamNull(string name)
		=> new() { Name = name, Value = new Field { IsNull = true } };
}
