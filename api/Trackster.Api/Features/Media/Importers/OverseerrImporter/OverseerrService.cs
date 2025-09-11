using Newtonsoft.Json;

namespace Trackster.Api.Features.Media.Importers.OverseerrImporter;

public class OverseerrService
{
    private readonly string _baseUrl;

    public OverseerrService()    
    {
        _baseUrl = Environment.GetEnvironmentVariable("ASPNETCORE_OVERSEERR_URL")!;
    }

    public async Task<List<string>> GetPosters()
    {
        var baseAddress = new Uri(_baseUrl);

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            using (var response = await httpClient.GetAsync($"api/v1/backdrops"))
            {
                string responseData = await response.Content.ReadAsStringAsync();

                try
                {
                    var parsedData = JsonConvert.DeserializeObject<List<string>>(responseData);
                    return parsedData ?? new List<string>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[FATAL] - Failed to get posters. Error: {ex.Message} Response below:");
                    Console.WriteLine(responseData);
                }

                return new List<string>();
            }
        }
    }
}