using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class LinqConfigureExtensions
{
	// Throws if the section is missing
	public static IConfigurationSection GetRequiredSection(this IConfiguration config, string key)
	{
		var section = config.GetSection(key);
		if (!section.Exists())
			throw new InvalidOperationException($"Configuration key '{key}' not found.");
		return section;
	}

	// Binds a section to a new instance of T; throws if missing
	public static T GetRequiredBindSection<T>(this IConfiguration config, string key) where T : class, new()
	{
		var section = config.GetSection(key);
		if (!section.Exists())
			throw new InvalidOperationException($"Section '{key}' not found in configuration.");
		var result = new T();
		section.Bind(result);
		return result;
	}

	// Copies filePath into AppContext.BaseDirectory and builds an IConfigurationRoot from it
	public static IConfigurationRoot BuildFromFile(string filePath)
	{
		if (File.Exists(filePath))
		{
			string dest = Path.Combine(AppContext.BaseDirectory, Path.GetFileName(filePath));
			File.Copy(filePath, dest, overwrite: true);
		}
		return new ConfigurationBuilder()
			.SetBasePath(AppContext.BaseDirectory)
			.AddJsonFile("appsettings.json")
			.Build();
	}
}

// Reads config values with ENV override: env variable takes priority, then appsettings, then default.
public class ConfigurationHelper
{
	private readonly IConfiguration _config;
	public ConfigurationHelper(IConfiguration config) => _config = config;

	public bool GetBool(string configKey, string envKey, bool defaultValue)
	{
		string val = Environment.GetEnvironmentVariable(envKey);
		if (string.IsNullOrEmpty(val))
			return _config.GetValue<bool>(configKey, defaultValue);
		return bool.TryParse(val, out bool r) ? r : defaultValue;
	}

	public string GetString(string configKey, string envKey, string defaultValue)
		=> Environment.GetEnvironmentVariable(envKey)
		   ?? _config[configKey]
		   ?? defaultValue;

	public string GetConnectionString(string configKey, string envKey, string defaultValue)
		=> Environment.GetEnvironmentVariable(envKey)
		   ?? _config.GetConnectionString(configKey)
		   ?? defaultValue;
}
