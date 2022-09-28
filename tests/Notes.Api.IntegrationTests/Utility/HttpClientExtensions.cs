using System.Net.Http.Headers;
using System.Text.Json;

namespace Notes.Api.IntegrationTests.Utility;

public static class HttpClientExtensions
{
    public static async Task<HttpResponseMessage> DeleteWithJsonAsync<T>(this HttpClient httpClient, string endpoint, T input)
    {
        var serialized = JsonSerializer.Serialize(input);
        var uri = $"{httpClient.BaseAddress}{endpoint}";
        var request = new HttpRequestMessage
        {
            Method = new HttpMethod("DELETE"),
            RequestUri = new Uri(uri),
            Content = new StringContent(serialized)
        };
        request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/json");
        return await httpClient.SendAsync(request);
    }
}