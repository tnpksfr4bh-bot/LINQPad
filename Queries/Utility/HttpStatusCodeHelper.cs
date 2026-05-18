using System.Net;
using System.Text.RegularExpressions;

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
