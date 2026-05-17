using System.Collections.Concurrent;

namespace Common;

// Target: .NET Standard 2.1 | .NET 8 | .NET 10
// In-process locale/country string table. Key format: {id}__{LOCALE}__{COUNTRY}
// Falls back to ko/kr when the requested locale is not available.
public static class StringTableCache
{
	// TTL per entry is 5 minutes
	private static readonly ConcurrentDictionary<string, (string text, DateTime expiresAt)> _table = new();

	private static string MakeKey(string id, string locale, string country)
		=> $"{id}__{locale.ToUpper()}__{country.ToUpper()}";

	public static void Set(string compositeKey, string text)
	{
		if (!string.IsNullOrEmpty(compositeKey))
			_table[compositeKey] = (text, DateTime.UtcNow.AddSeconds(300));
	}

	public static string Get(string id, string locale = "en", string country = "")
	{
		if (string.IsNullOrEmpty(id)) return string.Empty;

		(string normalizedLocale, string normalizedCountry) = Normalize(locale, country);
		string key = MakeKey(id, normalizedLocale, normalizedCountry);

		if (_table.TryGetValue(key, out var entry) && !string.IsNullOrEmpty(entry.text))
			return entry.text;

		// Fallback to Korean
		string fallbackKey = MakeKey(id, "ko", "kr");
		if (_table.TryGetValue(fallbackKey, out var fallback) && !string.IsNullOrEmpty(fallback.text))
			return fallback.text;

		return id; // key itself as last resort
	}

	private static (string locale, string country) Normalize(string locale, string country)
	{
		if (country.Equals("tw", StringComparison.OrdinalIgnoreCase))         return ("zh", "tw");
		if (locale.Equals("zh", StringComparison.OrdinalIgnoreCase)
		 || country.Equals("cn", StringComparison.OrdinalIgnoreCase))          return ("zh", "cn");
		if (locale.Equals("ja", StringComparison.OrdinalIgnoreCase)
		 || country.Equals("jp", StringComparison.OrdinalIgnoreCase))          return ("ja", "jp");
		if (locale.Equals("ko", StringComparison.OrdinalIgnoreCase)
		 || country.Equals("kr", StringComparison.OrdinalIgnoreCase))          return ("ko", "kr");
		return ("en", "us");
	}
}
