namespace Common.Util;

public static class HttpUtil
{
    static readonly HttpClient client = new();

    public static async Task<HttpResponseMessage?> GetAsync(string url)
    {
        try
        {
            var response = await client.GetAsync(url);
            response.EnsureSuccessStatusCode();
            return response;
        }
        catch (HttpRequestException e)
        {
            Console.WriteLine("\nException Caught!");
            Console.WriteLine($"Message :{e.Message}");
            return null;
        }
    }

    public static HttpResponseMessage Get(string url) => GetAsync(url).Result!;
    public static string GetString(string url) => Get(url).Content.ReadAsStringAsync().Result;
    public static Stream GetStream(string url) => Get(url).Content.ReadAsStreamAsync().Result;
}
