namespace Trackster.Api.Features.Notifications;

public class NotificationsService
{
    private readonly HttpClient _httpClient;

    public NotificationsService(HttpClient? httpClient = null)
    {
        _httpClient = httpClient ?? new HttpClient { BaseAddress = new Uri("https://api.pushover.net/1/") };
    }

    public async Task Send(string message)
    {
        var token = Environment.GetEnvironmentVariable("ASPNETCORE_PUSHOVER_TOKEN") ?? string.Empty;
        var user = Environment.GetEnvironmentVariable("ASPNETCORE_PUSHOVER_USER") ?? string.Empty;

        using (var content = new FormUrlEncodedContent(new Dictionary<string, string>
               {
                   ["token"] = token,
                   ["user"] = user,
                   ["message"] = message
               }))
        {
            using (var response = await _httpClient.PostAsync("messages.json", content))
            {
                var responseData = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"[INFO] - Received response from Pushovers {responseData}.");
            }
        }
    }
}