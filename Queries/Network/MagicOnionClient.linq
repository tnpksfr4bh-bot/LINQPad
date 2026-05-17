<Query Kind="Program">
  <NuGetReference>MagicOnion.Client</NuGetReference>
  <NuGetReference>MessagePack</NuGetReference>
  <Namespace>Grpc.Core</Namespace>
  <Namespace>Grpc.Net.Client</Namespace>
  <Namespace>MagicOnion</Namespace>
  <Namespace>MagicOnion.Client</Namespace>
  <Namespace>MagicOnion.Serialization</Namespace>
  <Namespace>MessagePack</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

// --- Test ---
// Replace IMyHub / IMyReceiver with your generated interfaces, then:
//
//   var client = new MagicOnionHubClient<IMyHub, IMyReceiver>(
//       "https://localhost:5001",
//       new MyReceiver(),
//       headers: new() { ["authorization"] = "Bearer TOKEN" });
//
//   await client.ConnectAsync();
//   await client.Hub.SomeMethodAsync("hello");
//   await client.DisposeAsync();
//
async Task Main()
{
	Console.WriteLine("Replace IMyHub / IMyReceiver with your generated interfaces.");
}

// ---

// Generic MagicOnion StreamingHub client wrapper.
// THub    = your IStreamingHub<THub, TReceiver> interface
// TReceiver = your IStreamingHubReceiver interface
public class MagicOnionHubClient<THub, TReceiver> : IAsyncDisposable
	where THub     : IStreamingHub<THub, TReceiver>
	where TReceiver : IStreamingHubReceiver
{
	private readonly GrpcChannel   _channel;
	private readonly CallInvoker   _invoker;
	private readonly TReceiver     _receiver;
	private readonly Dictionary<string, string> _headers;
	private readonly CancellationTokenSource    _cts = new();

	private THub _hub;

	public THub Hub => _hub;

	public MagicOnionHubClient(string address, TReceiver receiver,
		Dictionary<string, string> headers = null)
	{
		_channel  = GrpcChannel.ForAddress(address);
		_invoker  = _channel.CreateCallInvoker();
		_receiver = receiver;
		_headers  = headers ?? new();
	}

	public async Task ConnectAsync()
	{
		var metadata = BuildMetadata();
		var options  = new CallOptions()
			.WithHeaders(metadata)
			.WithCancellationToken(_cts.Token)
			.WithDeadline(DateTime.MaxValue)
			.WithWaitForReady();

		if (!StreamingHubClientFactoryProvider.Default
		    .TryGetFactory<THub, TReceiver>(out var factory))
			throw new NotSupportedException($"No StreamingHub factory for {typeof(THub).Name}");

		var hub = (StreamingHubClientBase<THub, TReceiver>)factory(
			_invoker, _receiver, null, options,
			MagicOnionSerializerProvider.Default, null);

		await hub.__ConnectAndSubscribeAsync(_receiver, _cts.Token);
		_hub = (THub)(object)hub;
	}

	// Creates a plain unary MagicOnion service client (non-streaming)
	public TService CreateService<TService>() where TService : IService<TService>
		=> MagicOnionClient.Create<TService>(_invoker,
		       MagicOnionSerializerProvider.Default,
		       Array.Empty<IClientFilter>(),
		       MagicOnionClientFactoryProvider.Default);

	public void AddHeader(string key, string value) => _headers[key] = value;

	public async ValueTask DisposeAsync()
	{
		_cts.Cancel();
		if (_hub is IAsyncDisposable d) await d.DisposeAsync();
		_channel.Dispose();
	}

	private Metadata BuildMetadata()
	{
		var m = new Metadata();
		foreach (var kv in _headers)
			m.Add(kv.Key, kv.Value);
		return m;
	}
}
