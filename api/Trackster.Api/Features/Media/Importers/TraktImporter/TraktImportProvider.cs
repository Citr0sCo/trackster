using System.Net;
using Newtonsoft.Json;
using Trackster.Api.Features.Auth.Providers.Trakt;
using Trackster.Api.Features.Media.Importers.TraktImporter.Types;
using Trackster.Api.Features.Sessions;
using Trackster.Api.Features.Users;

namespace Trackster.Api.Features.Media.Importers.TraktImporter;

public class TraktImportProvider
{
    private const int PageLimit = 100;
    private const int MaxRateLimitRetries = 5;

    private readonly string _clientId;
    private readonly TraktAuthProvider _traktAuthProvider;

    public TraktImportProvider(IUsersService usersService, ISessionService sessionService)
    {
        _clientId = Environment.GetEnvironmentVariable("ASPNETCORE_TRAKT_CLIENT_ID")!;
        _traktAuthProvider = new TraktAuthProvider(usersService, sessionService);
    }

    public Task<List<TraktMovieResponse>> GetMovies(string username, bool requestDebug)
    {
        return GetAllPages<TraktMovieResponse>(username, $"users/{username}/watched/movies", "movies", requestDebug);
    }

    public Task<List<TraktShowResponse>> GetShows(string username, bool requestDebug)
    {
        return GetAllPages<TraktShowResponse>(username, $"users/{username}/watched/shows", "shows", requestDebug);
    }

    public Task<List<TraktMovieHistoryResponse>> GetWatchedMovieHistory(string username, string itemId)
    {
        return GetAllPages<TraktMovieHistoryResponse>(username, $"users/{username}/history/movies/{itemId}", "movie history", false);
    }

    public Task<List<TraktShowHistoryResponse>> GetWatchedShowHistory(string username, string itemId)
    {
        return GetAllPages<TraktShowHistoryResponse>(username, $"users/{username}/history/shows/{itemId}", "show history", false);
    }

    private async Task<List<T>> GetAllPages<T>(string username, string path, string label, bool requestDebug)
    {
        var accessToken = await _traktAuthProvider.GetTokenByUsername(username);

        using var httpClient = CreateClient(accessToken);

        var results = new List<T>();
        var page = 1;

        while (true)
        {
            using var response = await SendWithRateLimitRetry(
                () => httpClient.GetAsync($"{path}?page={page}&limit={PageLimit}"),
                label);

            var responseData = await response.Content.ReadAsStringAsync();

            if (requestDebug)
                Console.WriteLine($"[DEBUG] - Received response from Trakt for {label} (page {page}) {responseData}.");

            if (!response.IsSuccessStatusCode)
                throw new InvalidOperationException($"Trakt returned {(int)response.StatusCode} ({response.StatusCode}) for {label} (page {page}). Body: {responseData}");

            List<T>? parsedData;

            try
            {
                parsedData = JsonConvert.DeserializeObject<List<T>>(responseData);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException($"Failed to parse the Trakt response for {label} (page {page}).", ex);
            }

            if (parsedData == null || parsedData.Count == 0)
                break;

            results.AddRange(parsedData);

            var pageCount = ReadPageCount(response);

            if (pageCount.HasValue)
            {
                if (page >= pageCount.Value)
                    break;
            }
            else if (parsedData.Count < PageLimit)
            {
                break;
            }

            page++;
        }

        return results;
    }

    private static async Task<HttpResponseMessage> SendWithRateLimitRetry(Func<Task<HttpResponseMessage>> sendRequest, string label)
    {
        for (var attempt = 0; ; attempt++)
        {
            var response = await sendRequest();

            if (response.StatusCode != HttpStatusCode.TooManyRequests || attempt >= MaxRateLimitRetries)
                return response;

            var retryAfter = response.Headers.RetryAfter?.Delta?.TotalSeconds ?? 60;

            Console.WriteLine($"[WARN] - Hitting Trakt throttle limit while fetching {label}. Waiting {retryAfter} seconds before retrying...");
            response.Dispose();

            await Task.Delay(TimeSpan.FromSeconds(Math.Max(1, retryAfter)));
        }
    }

    private static int? ReadPageCount(HttpResponseMessage response)
    {
        if (!response.Headers.TryGetValues("X-Pagination-Page-Count", out var values))
            return null;

        return int.TryParse(values.FirstOrDefault(), out var pageCount) ? pageCount : null;
    }

    private HttpClient CreateClient(string accessToken)
    {
        var httpClient = new HttpClient { BaseAddress = new Uri("https://api.trakt.tv/") };

        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", "2");
        httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", _clientId);
        httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
        httpClient.DefaultRequestHeaders.Add("User-Agent", "Trackster/1.0 (+https://trackster.miloszdura.com/)");

        return httpClient;
    }
}
