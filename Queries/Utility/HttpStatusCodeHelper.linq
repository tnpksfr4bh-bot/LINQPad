<Query Kind="Program">
  <Namespace>System.Net</Namespace>
</Query>

#load "HttpStatusCodeHelper.cs"

// --- Test ---
void Main()
{
	Console.WriteLine(HttpStatusCode.NotFound.ToSnakeCaseString());            // NOT_FOUND
	Console.WriteLine(HttpStatusCode.InternalServerError.ToSnakeCaseString()); // INTERNAL_SERVER_ERROR

	Console.WriteLine("--- All status codes ---");
	foreach (var name in HttpStatusCodeHelper.AllSnakeCaseNames())
		Console.WriteLine(name);
}
