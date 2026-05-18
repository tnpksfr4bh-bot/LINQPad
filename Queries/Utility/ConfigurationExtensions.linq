<Query Kind="Program">
  <Namespace>Microsoft.Extensions.Configuration</Namespace>
  <Namespace>Microsoft.Extensions.DependencyInjection</Namespace>
  <IncludeAspNet>true</IncludeAspNet>
</Query>

#load "ConfigurationExtensions.cs"

// --- Test ---
void Main()
{
	// Write a temp appsettings.json for testing
	string json = """{"MyKey":"hello","Db":{"Server":"localhost"}}""";
	string tmp  = Path.Combine(Path.GetTempPath(), "appsettings.json");
	File.WriteAllText(tmp, json);

	var config = new ConfigurationBuilder()
		.SetBasePath(Path.GetTempPath())
		.AddJsonFile("appsettings.json")
		.Build();

	Console.WriteLine(config.GetRequiredSection("MyKey").Value);           // hello
	Console.WriteLine(config.GetRequiredSection("Db")["Server"]);          // localhost

	var ext = new ConfigurationHelper(config);
	Console.WriteLine(ext.GetString("MyKey", "MY_KEY_ENV", "default"));    // hello
}
