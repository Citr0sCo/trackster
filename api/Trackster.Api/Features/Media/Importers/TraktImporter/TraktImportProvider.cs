using Newtonsoft.Json;
using Trackster.Api.Features.Media.Importers.TraktImporter.Types;

namespace Trackster.Api.Features.Media.Importers.TraktImporter;

public class TraktImportProvider
{
    private readonly string _apiKey = "ce40409023d4a567b678e19aa3c4b4dc243d05f85ac624f4d203840227043011";

    public async Task<List<TraktMovieResponse>> GetMovies(string username)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", _apiKey);

            using (var response = await httpClient.GetAsync($"users/{username}/watched/movies"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<List<TraktMovieResponse>>(responseData);
                return parsedData ?? new List<TraktMovieResponse>();
            }
        }
    }

    public async Task<List<TraktShowResponse>> GetShows(string username)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", _apiKey);

            using (var response = await httpClient.GetAsync($"users/{username}/watched/shows"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<List<TraktShowResponse>>(responseData);
                return parsedData ?? new List<TraktShowResponse>();
            }
        }
    }

    public async Task<List<TraktMovieHistoryResponse>> GetWatchedMovieHistory(string username, string itemId)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", _apiKey);

            using (var response = await httpClient.GetAsync($"users/{username}/history/movies/{itemId}"))
            {
                string responseData = await response.Content.ReadAsStringAsync();

                if (responseData.StartsWith("UNAUTHED_API_GET_LIMIT"))
                {
                    Console.WriteLine($"[WARN] - Hitting Trakt throttle limit. Waiting 60 seconds before continuing...");
                    Thread.Sleep(1000 * 60);
                    return new List<TraktMovieHistoryResponse>();
                }
                
                try
                {
                    var parsedData = JsonConvert.DeserializeObject<List<TraktMovieHistoryResponse>>(responseData);
                    return parsedData ?? new List<TraktMovieHistoryResponse>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[FATAL] - Failed to get movie watch history. Error: {ex.Message} Response below:");
                    Console.WriteLine(responseData);
                }
                
                return new List<TraktMovieHistoryResponse>();
            }
        }
    }

    public async Task<List<TraktShowHistoryResponse>> GetWatchedShowHistory(string username, string itemId)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", _apiKey);

            using (var response = await httpClient.GetAsync($"users/{username}/history/shows/{itemId}"))
            {
                string responseData = await response.Content.ReadAsStringAsync();

                if (responseData.StartsWith("UNAUTHED_API_GET_LIMIT"))
                {
                    Console.WriteLine($"[WARN] - Hitting Trakt throttle limit. Waiting 60 seconds before continuing...");
                    Thread.Sleep(1000 * 60);
                    return new List<TraktShowHistoryResponse>();
                }
                
                try
                {
                    var parsedData = JsonConvert.DeserializeObject<List<TraktShowHistoryResponse>>(responseData);
                    return parsedData ?? new List<TraktShowHistoryResponse>();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"[FATAL] - Failed to get show watch history. Error: {ex.Message} Response below:");
                    Console.WriteLine(responseData);
                }

                return new List<TraktShowHistoryResponse>();
            }
        }
    }
}