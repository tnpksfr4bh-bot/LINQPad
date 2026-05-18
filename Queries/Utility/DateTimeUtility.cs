public static class DateTimeUtility
{
	// Current UTC as Unix seconds
	public static long UtcNowSeconds()
		=> DateTimeOffset.UtcNow.ToUnixTimeSeconds();

	// Current UTC as Unix milliseconds
	public static long UtcNowMillis()
		=> DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

	// Convert Unix seconds → UTC DateTime
	public static DateTime FromSeconds(long unixSeconds)
		=> DateTimeOffset.FromUnixTimeSeconds(unixSeconds).UtcDateTime;

	// Convert Unix milliseconds → UTC DateTime
	public static DateTime FromMillis(long unixMillis)
		=> DateTimeOffset.FromUnixTimeMilliseconds(unixMillis).UtcDateTime;

	// Midnight of today (UTC) as Unix seconds
	public static long TodayMidnightSeconds()
	{
		var now = DateTime.UtcNow;
		var midnight = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc);
		return ((DateTimeOffset)midnight).ToUnixTimeSeconds();
	}

	// Midnight of the next calendar day (UTC) as Unix seconds
	public static long NextMidnightSeconds()
	{
		var now = DateTime.UtcNow;
		var next = new DateTime(now.Year, now.Month, now.Day, 0, 0, 0, DateTimeKind.Utc).AddDays(1);
		return ((DateTimeOffset)next).ToUnixTimeSeconds();
	}

	// Ceiling number of days between two DateTimes (used for reward cycles, etc.)
	public static int DaysCeiling(DateTime from, DateTime to)
		=> (int)Math.Ceiling((to - from).TotalDays);

	// Midnight of the day that contains a given Unix-seconds timestamp
	public static long MidnightOf(long unixSeconds)
	{
		var dt = FromSeconds(unixSeconds);
		var midnight = new DateTime(dt.Year, dt.Month, dt.Day, 0, 0, 0, DateTimeKind.Utc);
		return ((DateTimeOffset)midnight).ToUnixTimeSeconds();
	}
}
