<Query Kind="Program">
  <NuGetReference>Grpc.Net.Client</NuGetReference>
  <NuGetReference>Grpc.Health.V1</NuGetReference>
  <Namespace>Grpc.Health.V1</Namespace>
  <Namespace>Grpc.Net.Client</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "GrpcHealthCheckClient.cs"

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
