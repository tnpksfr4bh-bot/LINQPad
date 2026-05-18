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

#load "MagicOnionHubClient.cs"

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
