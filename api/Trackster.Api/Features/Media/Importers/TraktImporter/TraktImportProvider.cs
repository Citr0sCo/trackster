using Newtonsoft.Json;
using Trackster.Api.Features.Media.Importers.TraktImporter.Types;

namespace Trackster.Api.Features.Media.Importers.TraktImporter;

public class TraktImportProvider
{
    public async Task<List<TraktMovieResponse>> GetMovies(string username)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
        {
            //var traktClientId = Environment.GetEnvironmentVariable("ASPNETCORE_TRAKT_CLIENT_ID");
            
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", "ce40409023d4a567b678e19aa3c4b4dc243d05f85ac624f4d203840227043011");
  
            using(var response = await httpClient.GetAsync($"users/{username}/watched/movies"))
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

        using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", "ce40409023d4a567b678e19aa3c4b4dc243d05f85ac624f4d203840227043011");
  
            using(var response = await httpClient.GetAsync($"users/{username}/watched/shows"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<List<TraktShowResponse>>(responseData);
                return parsedData ?? new List<TraktShowResponse>();
            }
        }
    }
}