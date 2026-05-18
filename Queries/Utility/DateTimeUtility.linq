<Query Kind="Program" />

#load "DateTimeUtility.cs"

// --- Test ---
void Main()
{
	Console.WriteLine($"UtcNow seconds : {DateTimeUtility.UtcNowSeconds()}");
	Console.WriteLine($"UtcNow millis  : {DateTimeUtility.UtcNowMillis()}");

	long ts = DateTimeUtility.UtcNowSeconds();
	Console.WriteLine($"From seconds   : {DateTimeUtility.FromSeconds(ts):s}");

	Console.WriteLine($"Today midnight : {DateTimeUtility.TodayMidnightSeconds()}");

	DateTime a = new DateTime(2024, 5, 9);
	DateTime b = new DateTime(2024, 5, 10);
	Console.WriteLine($"Days ceiling   : {DateTimeUtility.DaysCeiling(a, b)}"); // 1
}
