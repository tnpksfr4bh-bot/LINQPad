<Query Kind="Program">
  <NuGetReference>Grpc.Net.Client</NuGetReference>
  <NuGetReference>Grpc.Health.V1</NuGetReference>
  <Namespace>Grpc.Health.V1</Namespace>
  <Namespace>Grpc.Net.Client</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

// --- Test ---
async Task Main()
{
	var checker = new GrpcHealthCheckClient("https://localhost:5001");

	bool alive = await checker.IsAliveAsync();
	Console.WriteLine($"Alive : {alive}");

	var status = await checker.CheckAsync();
	Console.WriteLine($"Status: {status}");

	// Named service check
	var svcStatus = await checker.CheckAsync("my.service.Name");
	Console.WriteLine($"Service status: {svcStatus}");
}

// ---

// Wraps the standard gRPC health-check protocol (grpc.health.v1.Health).
public class GrpcHealthCheckClient
{
	private readonly GrpcChannel _channel;
	private readonly Health.HealthClient _client;

	public GrpcHealthCheckClient(string address)
	{
		_channel = GrpcChannel.ForAddress(address);
		_client  = new Health.HealthClient(_channel);
	}

	// Returns SERVING / NOT_SERVING / UNKNOWN / SERVICE_UNKNOWN
	public async Task<HealthCheckResponse.Types.ServingStatus> CheckAsync(
		string service = "", CancellationToken ct = default)
	{
		try
		{
			var response = await _client.CheckAsync(
				new HealthCheckRequest { Service = service },
				cancellationToken: ct);
			return response.Status;
		}
		catch (Exception ex)
		{
			Debug.WriteLine($"[GrpcHealthCheck] {ex.Message}");
			return HealthCheckResponse.Types.ServingStatus.Unknown;
		}
	}

	public async Task<bool> IsAliveAsync(string service = "", CancellationToken ct = default)
		=> await CheckAsync(service, ct) == HealthCheckResponse.Types.ServingStatus.Serving;

	// Streaming watch — calls onStatus each time the serving state changes
	public async Task WatchAsync(
		Action<HealthCheckResponse.Types.ServingStatus> onStatus,
		string service = "",
		CancellationToken ct = default)
	{
		using var call = _client.Watch(
			new HealthCheckRequest { Service = service },
			cancellationToken: ct);

		await foreach (var response in call.ResponseStream.ReadAllAsync(ct))
			onStatus(response.Status);
	}
}
