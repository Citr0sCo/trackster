using Newtonsoft.Json;
using Trackster.Api.Features.Auth.Types;

namespace Trackster.Api.Features.Auth.Providers.Trakt;

public class TraktAuthProvider : IAuthProvider
{
    private readonly string? _clientId;
    private readonly string? _clientSecret;
    private readonly string? _baseUri;
    public bool IsActive { get; } = true;

    public TraktAuthProvider()
    {
        _clientId = Environment.GetEnvironmentVariable("ASPNETCORE_TRAKT_CLIENT_ID");
        _clientSecret = Environment.GetEnvironmentVariable("ASPNETCORE_TRAKT_CLIENT_SECRET");
        _baseUri = Environment.GetEnvironmentVariable("ASPNETCORE_BASE_URL");
        
        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            _baseUri = "http://localhost:4200";
    }
    
    public async Task<SignInResponse> SignIn(SignInRequest request)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            var body = new
            {
                code = request.Code,
                client_id = _clientId,
                client_secret = _clientSecret,
                redirect_uri = $"{_baseUri}/authorize/trakt",
                grant_type = "authorization_code",
            };

            using (var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.Default, "application/json"))
            {
                using (var response = await httpClient.PostAsync("oauth/token", content))
                {
                    var responseData = await response.Content.ReadAsStringAsync();
                    var parsedData = JsonConvert.DeserializeObject<TraktAuthResponse>(responseData);
                }
            }
        }

        return new SignInResponse();
    }

    public Task<RegisterResponse> Register(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<TraktProfileResponse?> GetProfile(string username = "citr0s")
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", _clientId);
  
            using(var response = await httpClient.GetAsync($"users/{username}"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<TraktProfileResponse>(responseData);
                return parsedData;
            }
        }
    }

    public async void SignOut(SignOutRequest request)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            var body = new
            {
                token = request.Token,
                client_id = _clientId,
                client_secret = _clientSecret
            };

            using (var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.Default, "application/json"))
            {
                using (var response = await httpClient.PostAsync("oauth/revoke", content))
                {
                    await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}