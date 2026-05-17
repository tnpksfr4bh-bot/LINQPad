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

// ---

// ---------- DbQuery (Dapper raw SQL) ----------

public class DbQuery
{
	public string Sql        { get; }
	public object Parameters { get; }

	public DbQuery(string sql, object parameters = null)
	{
		Sql        = sql;
		Parameters = parameters;
	}

	public async Task<IEnumerable<T>> ReaderAsync<T>(string connectionString)
	{
		using var conn = new MySqlConnector.MySqlConnection(connectionString);
		return await conn.QueryAsync<T>(Sql, Parameters);
	}

	public async Task<T> ScalarAsync<T>(string connectionString)
	{
		using var conn = new MySqlConnector.MySqlConnection(connectionString);
		return await conn.ExecuteScalarAsync<T>(Sql, Parameters);
	}

	public async Task<int> AffectedAsync(string connectionString)
	{
		using var conn = new MySqlConnector.MySqlConnection(connectionString);
		return await conn.ExecuteAsync(Sql, Parameters);
	}

	public async Task<long> LastInsertedIdAsync(string connectionString)
	{
		using var conn = new MySqlConnector.MySqlConnection(connectionString);
		await conn.ExecuteAsync(Sql, Parameters);
		return await conn.ExecuteScalarAsync<long>("SELECT LAST_INSERT_ID()");
	}
}

// ---------- DbQueryLogger (slow query interceptor) ----------

public class DbQueryLogger : DbCommandInterceptor
{
	private readonly int _thresholdMs;
	public DbQueryLogger(int thresholdMs = 500) => _thresholdMs = thresholdMs;

	public override DbDataReader ReaderExecuted(DbCommand command, CommandExecutedEventData data, DbDataReader result)
	{
		Log(command.CommandText, data.Duration.TotalMilliseconds);
		return result;
	}

	public override ValueTask<DbDataReader> ReaderExecutedAsync(DbCommand command, CommandExecutedEventData data, DbDataReader result, CancellationToken ct = default)
	{
		Log(command.CommandText, data.Duration.TotalMilliseconds);
		return new ValueTask<DbDataReader>(result);
	}

	private void Log(string sql, double ms)
	{
		if (ms >= _thresholdMs)
			Debug.WriteLine($"[SlowQuery {ms:F0}ms] {sql}");
	}
}

// ---------- IDbExtension / DbExtension<T> ----------

public interface IDbExtension<TContext> where TContext : DbContext
{
	TContext Db { get; }
	Task<int>  SaveAsync(CancellationToken ct = default);
	Task<T>    ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken ct = default);
	Task       ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default);
}

public class DbExtension<TContext> : IDbExtension<TContext> where TContext : DbContext
{
	public TContext Db { get; }
	public DbExtension(TContext db) => Db = db;

	public Task<int> SaveAsync(CancellationToken ct = default)
		=> Db.SaveChangesAsync(ct);

	public async Task<T> ExecuteInTransactionAsync<T>(Func<Task<T>> action, CancellationToken ct = default)
	{
		var strategy = Db.Database.CreateExecutionStrategy();
		return await strategy.ExecuteAsync(async () =>
		{
			await using var tx = await Db.Database.BeginTransactionAsync(ct);
			var result = await action();
			await tx.CommitAsync(ct);
			return result;
		});
	}

	public async Task ExecuteInTransactionAsync(Func<Task> action, CancellationToken ct = default)
	{
		var strategy = Db.Database.CreateExecutionStrategy();
		await strategy.ExecuteAsync(async () =>
		{
			await using var tx = await Db.Database.BeginTransactionAsync(ct);
			await action();
			await tx.CommitAsync(ct);
		});
	}
}

// ---------- DbContext definitions ----------
// Each context maps to one MySQL schema. Add/remove entity sets as needed.

public class AppDbContext : DbContext
{
	public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }
	// public DbSet<User> Users { get; set; }
}

public class ServiceDbContext : DbContext
{
	public ServiceDbContext(DbContextOptions<ServiceDbContext> options) : base(options) { }
}

public class CmsDbContext : DbContext
{
	public CmsDbContext(DbContextOptions<CmsDbContext> options) : base(options) { }
}

public class GameDbContext : DbContext
{
	public GameDbContext(DbContextOptions<GameDbContext> options) : base(options) { }
}

public class IapDbContext : DbContext
{
	public IapDbContext(DbContextOptions<IapDbContext> options) : base(options) { }
}

public class UserDbContext : DbContext
{
	public UserDbContext(DbContextOptions<UserDbContext> options) : base(options) { }
}

public class SnsDbContext : DbContext
{
	public SnsDbContext(DbContextOptions<SnsDbContext> options) : base(options) { }
}
