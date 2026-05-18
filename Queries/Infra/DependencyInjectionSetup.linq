<Query Kind="Program">
  <Namespace>Microsoft.AspNetCore.Builder</Namespace>
  <Namespace>Microsoft.AspNetCore.Diagnostics.HealthChecks</Namespace>
  <Namespace>Microsoft.AspNetCore.Http</Namespace>
  <Namespace>Microsoft.AspNetCore.Routing</Namespace>
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection.Extensions</Namespace>
  <Namespace>Microsoft.Extensions.Diagnostics.HealthChecks</Namespace>
  <Namespace>Microsoft.Extensions.Hosting</Namespace>
  <Namespace>System.Threading.Tasks</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "DependencyInjectionSetup.cs"

// --- Test ---
void Main()
{
	var services = new ServiceCollection();
	services.AddMemoryCache();
	services.AddHealthCheckExtension();

	var provider = services.BuildServiceProvider();
	Console.WriteLine("DI container built successfully.");
}
