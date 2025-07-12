using Newtonsoft.Json;
using Trackster.Api.Features.Media.Importers.TmdbImporter.Types;

namespace Trackster.Api.Features.Media.Importers.TmdbImporter;

public class TmdbImportProvider
{
    public async Task<TmdbMovieDetails> GetDetailsForMovie(string reference)
    {
        var baseAddress = new Uri("https://api.themoviedb.org/");

        using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
        {
            //var traktClientId = Environment.GetEnvironmentVariable("ASPNETCORE_TRAKT_CLIENT_ID");
            
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJhODRjNDI4MjZmNWJkNjIxZThlNjkyMWMwZTYxZTAxZSIsIm5iZiI6MTUxNTY2NDUwMS43MTgwMDAyLCJzdWIiOiI1YTU3MzQ3NTBlMGEyNjA3ZDcwMzY0MWEiLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.iT-8sXm-DxqL5uXUrznWUZmn41UOeI6vGyby9ZSvMX4");
  
            using(var response = await httpClient.GetAsync($"3/movie/{reference}"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<TmdbMovieDetails>(responseData);
                return parsedData ?? new TmdbMovieDetails();
            }
        }
    }
    
    public async Task<TmdbShowDetails> GetDetailsForShow(string reference)
    {
        var baseAddress = new Uri("https://api.themoviedb.org/");

        using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
        {
            //var traktClientId = Environment.GetEnvironmentVariable("ASPNETCORE_TRAKT_CLIENT_ID");
            
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", "Bearer eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJhODRjNDI4MjZmNWJkNjIxZThlNjkyMWMwZTYxZTAxZSIsIm5iZiI6MTUxNTY2NDUwMS43MTgwMDAyLCJzdWIiOiI1YTU3MzQ3NTBlMGEyNjA3ZDcwMzY0MWEiLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.iT-8sXm-DxqL5uXUrznWUZmn41UOeI6vGyby9ZSvMX4");
  
            using(var response = await httpClient.GetAsync($"3/tv/{reference}"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<TmdbShowDetails>(responseData);
                return parsedData ?? new TmdbShowDetails();
            }
        }
    }
}