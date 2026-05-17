<Query Kind="Program">
  <Namespace>System.Net</Namespace>
</Query>

// --- Test ---
void Main()
{
	Console.WriteLine(HttpStatusCode.NotFound.ToSnakeCaseString());        // NOT_FOUND
	Console.WriteLine(HttpStatusCode.InternalServerError.ToSnakeCaseString()); // INTERNAL_SERVER_ERROR

	Console.WriteLine("--- All status codes ---");
	foreach (var name in HttpStatusCodeHelper.AllSnakeCaseNames())
		Console.WriteLine(name);
}

// ---

public static class HttpStatusCodeHelper
{
	public static IEnumerable<string> AllSnakeCaseNames()
		=> Enum.GetNames(typeof(HttpStatusCode)).Select(ToSnakeCase);

	private static string ToSnakeCase(string name)
		=> Regex.Replace(name, "(?<=[a-z])(?=[A-Z])", "_").ToUpper();
}

public static class HttpStatusCodeExtensions
{
	public static string ToSnakeCaseString(this HttpStatusCode code)
		=> Regex.Replace(code.ToString(), "(?<=[a-z])(?=[A-Z])", "_").ToUpper();
}
