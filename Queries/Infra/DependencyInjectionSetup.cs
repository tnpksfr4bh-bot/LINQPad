using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using Microsoft.Extensions.Hosting;

// IServiceCollection extensions for common infrastructure registrations.
public static class ServiceCollectionExtensions
{
	// Registers startup health check + optional hosted background service.
	public static IServiceCollection AddHealthCheckExtension(
		this IServiceCollection services,
		bool includeStartupBackgroundService = false)
	{
		if (includeStartupBackgroundService)
			services.AddHostedService<StartupBackgroundService>();

		services.TryAddSingleton<StartupHealthCheck>();
		services.AddHealthChecks()
		        .AddCheck<StartupHealthCheck>("Startup", tags: new[] { "ready" });
		return services;
	}
}

// IEndpointRouteBuilder extensions for health check and Docker curl probe endpoints.
public static class EndpointRouteBuilderExtensions
{
	// Maps /healthz/ready + /healthz/live and optionally GET / for Docker curl health checks.
	// docker: CMD-SHELL,curl http://localhost:5000/ || exit 1
	public static void MapHealthCheckExtension(
		this IEndpointRouteBuilder endpoints,
		bool includeCurlRootEndpoint = true)
	{
		if (includeCurlRootEndpoint)
			endpoints.MapGet("/", async context => await context.Response.WriteAsync(string.Empty));

		endpoints.MapHealthChecks("/healthz/ready", new HealthCheckOptions
		{
			Predicate = hc => hc.Tags.Contains("ready")
		});

		endpoints.MapHealthChecks("/healthz/live", new HealthCheckOptions
		{
			Predicate = _ => false
		});
	}

	// Maps path base + static files (requires leading slash in pathBase)
	public static IApplicationBuilder UsePathChangeExtension(
		this IApplicationBuilder app, string pathBase)
		=> app.UseStaticFiles(pathBase).UsePathBase(pathBase);
}

// Signals startup completion to the health check.
public class StartupHealthCheck : IHealthCheck
{
	private volatile bool _ready;
	public bool StartupCompleted { get => _ready; set => _ready = value; }

	public Task<HealthCheckResult> CheckHealthAsync(
		HealthCheckContext ctx, CancellationToken ct = default)
		=> Task.FromResult(
			_ready
			? HealthCheckResult.Healthy("Startup complete.")
			: HealthCheckResult.Unhealthy("Startup in progress."));
}

// Drives startup completion via IHostedService.
public class StartupBackgroundService : BackgroundService
{
	private readonly StartupHealthCheck _health;
	public StartupBackgroundService(StartupHealthCheck health) => _health = health;

	protected override async Task ExecuteAsync(CancellationToken ct)
	{
		await Task.Delay(1, ct);
		_health.StartupCompleted = true;
	}
}

// Reads long ID generator used by server-side code.
public static class RandomHelper
{
	private static readonly Random _rng  = new();
	private static readonly object _lock = new();

	// Generates a time-prefixed long ID (not cryptographically secure).
	public static long GenerateLongId()
	{
		long ts     = (DateTime.UtcNow.Ticks - 621355968000000000L) / 10_000_000L;
		long prefix = (ts - 1_400_000_000L) / 3600L;
		lock (_lock)
		{
			int a = _rng.Next(10000, 99999);
			int b = _rng.Next(100000, 999999);
			return (long)ulong.Parse($"{prefix}{a}{b}");
		}
	}
}
