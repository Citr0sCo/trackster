using Newtonsoft.Json;
using Trackster.Api.Features.Media.Importers.TmdbImporter.Types;

namespace Trackster.Api.Features.Media.Importers.TmdbImporter;

public class TmdbImportProvider
{
    private readonly string _authToken;

    public TmdbImportProvider()
    {
        _authToken = "eyJhbGciOiJIUzI1NiJ9.eyJhdWQiOiJhODRjNDI4MjZmNWJkNjIxZThlNjkyMWMwZTYxZTAxZSIsIm5iZiI6MTUxNTY2NDUwMS43MTgwMDAyLCJzdWIiOiI1YTU3MzQ3NTBlMGEyNjA3ZDcwMzY0MWEiLCJzY29wZXMiOlsiYXBpX3JlYWQiXSwidmVyc2lvbiI6MX0.iT-8sXm-DxqL5uXUrznWUZmn41UOeI6vGyby9ZSvMX4";
    }
    
    public async Task<TmdbMovieDetails> GetDetailsForMovie(string reference)
    {
        var baseAddress = new Uri("https://api.themoviedb.org/");

        using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");
  
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
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");
  
            using(var response = await httpClient.GetAsync($"3/tv/{reference}"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<TmdbShowDetails>(responseData);
                return parsedData ?? new TmdbShowDetails();
            }
        }
    }

    public async Task<TmdbMovieSearchResults> FindMovieByTitleAndYear(string title, int year)
    {
        var baseAddress = new Uri("https://api.themoviedb.org/");

        using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");

            var queryParams = new Dictionary<string, string>
            {
                { "title", title }, 
                { "year", year.ToString() },
                { "include_adult", "false" },
                { "language", "en-US" }
            };
            
            var flatQueryParams = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            
            using(var response = await httpClient.GetAsync($"3/search/movie?{flatQueryParams}"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<TmdbMovieSearchResults>(responseData);
                return parsedData ?? new TmdbMovieSearchResults();
            }
        }
    }

    public async Task<TmdbShowSearchResults> FindShowByTitleAndYear(string title, int year)
    {
        var baseAddress = new Uri("https://api.themoviedb.org/");

        using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");

            var queryParams = new Dictionary<string, string>
            {
                { "title", title }, 
                { "year", year.ToString() },
                { "include_adult", "false" },
                { "language", "en-US" }
            };
            
            var flatQueryParams = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            
            using(var response = await httpClient.GetAsync($"3/search/show?{flatQueryParams}"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<TmdbShowSearchResults>(responseData);
                return parsedData ?? new TmdbShowSearchResults();
            }
        }
    }

    public async Task<TmdbSeasonSearchResults> GetDetailsForSeason(int showIdentifier, int seasonNumber)
    {
        var baseAddress = new Uri("https://api.themoviedb.org/");

        using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");

            var queryParams = new Dictionary<string, string>
            {
                { "language", "en-US" }
            };
            
            var flatQueryParams = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
            
            using(var response = await httpClient.GetAsync($"3/tv/{showIdentifier}/season/{seasonNumber}?{flatQueryParams}"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<TmdbSeasonSearchResults>(responseData);
                return parsedData ?? new TmdbSeasonSearchResults();
            }
        }
    }
}