using Newtonsoft.Json;
using Trackster.Api.Features.Media.Importers.TmdbImporter.Types;

namespace Trackster.Api.Features.Media.Importers.TmdbImporter;

public class TmdbImportProvider
{
    private readonly string _authToken;

    public TmdbImportProvider()
    {
        _authToken = Environment.GetEnvironmentVariable("ASPNETCORE_TMDB_API_KEY")!;
    }
    
    public async Task<TmdbMovieDetails> GetDetailsForMovie(string reference, bool requestDebug = false)
    {
        try
        {
            var baseAddress = new Uri("https://api.themoviedb.org/");

            using (var httpClient = new HttpClient { BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");

                using (var response = await httpClient.GetAsync($"3/movie/{reference}"))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    
                    if(requestDebug)
                        Console.WriteLine($"[DEBUG] - Received response from TMDB details for movie {responseData}.");
                    
                    var parsedData = JsonConvert.DeserializeObject<TmdbMovieDetails>(responseData);
                    return parsedData ?? new TmdbMovieDetails();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL] - Failed to get movie details. Exception: {ex.Message}.");
            Console.WriteLine(ex);
        }

        return new TmdbMovieDetails();
    }
    
    public async Task<TmdbShowDetails> GetDetailsForShow(string reference, bool requestDebug)
    {
        try 
        {
            Console.WriteLine($"[INFO] - Making request for details for Show Details by {reference}.");
            
            var baseAddress = new Uri("https://api.themoviedb.org/");

            using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");
      
                using(var response = await httpClient.GetAsync($"3/tv/{reference}"))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    
                    if(requestDebug)
                        Console.WriteLine($"[DEBUG] - Received response from TMDB details for show {responseData}.");

                    if (responseData == "The resource you requested could not be found.")
                        return new TmdbShowDetails();
                    
                    var parsedData = JsonConvert.DeserializeObject<TmdbShowDetails>(responseData);
                    return parsedData ?? new TmdbShowDetails();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL] - Failed to get show details. Exception: {ex.Message}.");
            Console.WriteLine(ex);
        }

        return new TmdbShowDetails();
    }
    
    public async Task<TmdbEpisodeDetails> GetEpisodeDetails(string seriesId, int seasonNumber, int episodeNumber, bool requestDebug)
    {
        try 
        {
            var baseAddress = new Uri("https://api.themoviedb.org/");

            using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");
      
                using(var response = await httpClient.GetAsync($"3/tv/{seriesId}/season/{seasonNumber}/episode/{episodeNumber}"))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    
                    if(requestDebug)
                        Console.WriteLine($"[DEBUG] - Received response from TMDB details for episode {responseData}.");
                    
                    var parsedData = JsonConvert.DeserializeObject<TmdbEpisodeDetails>(responseData);
                    return parsedData ?? new TmdbEpisodeDetails();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL] - Failed to get episode details. Exception: {ex.Message}.");
            Console.WriteLine(ex);
        }

        return new TmdbEpisodeDetails();
    }

    public async Task<TmdbMovieSearchResults> FindMovieByTitleAndYear(string title, int year, bool requestDebug)
    {
        try 
        {
            var baseAddress = new Uri("https://api.themoviedb.org/");

            using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");

                var queryParams = new Dictionary<string, string>
                {
                    { "query", title }, 
                    { "year", year.ToString() },
                    { "include_adult", "false" },
                    { "language", "en-US" }
                };
                
                var flatQueryParams = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                
                using(var response = await httpClient.GetAsync($"3/search/movie?{flatQueryParams}"))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    
                    if(requestDebug)
                        Console.WriteLine($"[DEBUG] - Received response from TMDB search for movie {responseData}.");
                    
                    var parsedData = JsonConvert.DeserializeObject<TmdbMovieSearchResults>(responseData);
                    return parsedData ?? new TmdbMovieSearchResults();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL] - Failed to find movie by title and year. Exception: {ex.Message}.");
            Console.WriteLine(ex);
        }
        
        return new  TmdbMovieSearchResults();
    }

    public async Task<TmdbShowSearchResults> FindShowByTitleAndYear(string title, int year, bool requestDebug)
    {
        try 
        {
            var baseAddress = new Uri("https://api.themoviedb.org/");

            using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
            {
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("accept", "application/json");
                httpClient.DefaultRequestHeaders.TryAddWithoutValidation("Authorization", $"Bearer {_authToken}");

                if (title.Contains("("))
                {
                    var explodedTitle = title.Split("(");
                    title = explodedTitle[0].Trim();

                    if(year == 0)
                        year = int.Parse(explodedTitle[1].Split(")")[0].Trim());
                    
                    Console.WriteLine($"[INFO] - Parsed title and got title: {title}, year: {year}.");
                }
                
                var queryParams = new Dictionary<string, string>
                {
                    { "query", title }, 
                    { "year", year.ToString() },
                    { "include_adult", "false" },
                    { "language", "en-US" }
                };
                
                var flatQueryParams = string.Join("&", queryParams.Select(kvp => $"{kvp.Key}={kvp.Value}"));
                
                using(var response = await httpClient.GetAsync($"3/search/tv?{flatQueryParams}"))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    
                    if(requestDebug)
                        Console.WriteLine($"[DEBUG] - Received response from TMDB search for show {responseData}.");
                    
                    var parsedData = JsonConvert.DeserializeObject<TmdbShowSearchResults>(responseData);
                    return parsedData ?? new TmdbShowSearchResults();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL] - Failed to find show by title and year. Exception: {ex.Message}.");
            Console.WriteLine(ex);
        }

        return new TmdbShowSearchResults();
    }

    public async Task<TmdbSeasonSearchResults> GetDetailsForSeason(int showIdentifier, int seasonNumber, bool requestDebug)
    {
        try 
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
                    
                    if(requestDebug)
                        Console.WriteLine($"[DEBUG] - Received response from TMDB details for season {responseData}.");
                    
                    var parsedData = JsonConvert.DeserializeObject<TmdbSeasonSearchResults>(responseData);
                    return parsedData ?? new TmdbSeasonSearchResults();
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL] - Failed to find details for season. Exception: {ex.Message}.");
            Console.WriteLine(ex);
        }

        return new TmdbSeasonSearchResults();
    }
}