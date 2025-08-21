using Newtonsoft.Json;

namespace Trackster.Api.Features.Notifications;

public class NotificationsService
{
    public async Task Send(string message)
    {
        var baseAddress = new Uri("https://api.pushover.net/1/");
        var token = Environment.GetEnvironmentVariable("ASPNETCORE_PUSHOVER_TOKEN");
        var user = Environment.GetEnvironmentVariable("ASPNETCORE_PUSHOVER_USER");
        
        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            var body = new { };

            using (var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.Default, "application/json"))
            {
                using (var response = await httpClient.PostAsync($"messages.json?token={token}&user={user}&message={message}", content))
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"[DEBUG] - 1/1 - Received response from Pushovers {responseData}.");
                }
            }
        }
    }
}