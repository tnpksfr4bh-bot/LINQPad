using Grpc.Health.V1;
using Grpc.Net.Client;

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
