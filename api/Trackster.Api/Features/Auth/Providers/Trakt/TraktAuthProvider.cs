using Newtonsoft.Json;
using Trackster.Api.Core.Types;
using Trackster.Api.Features.Auth.Providers.Trakt.Types;
using Trackster.Api.Features.Auth.Types;
using Trackster.Api.Features.Sessions;
using Trackster.Api.Features.Users;
using Trackster.Api.Features.Users.Types;

namespace Trackster.Api.Features.Auth.Providers.Trakt;

public class TraktAuthProvider : IAuthProvider
{
    private readonly UsersService _usersService;
    private readonly SessionService _sessionService;
    private readonly SessionFactory _sessionFactory;

    private readonly string? _clientId;
    private readonly string? _clientSecret;
    private readonly string? _baseUri;
    public bool IsActive { get; } = true;

    public TraktAuthProvider(UsersService usersService, SessionService sessionService)
    {
        _usersService = usersService;
        _sessionService = sessionService;
        _sessionFactory = SessionFactory.Instance();

        _clientId = Environment.GetEnvironmentVariable("ASPNETCORE_TRAKT_CLIENT_ID");
        _clientSecret = Environment.GetEnvironmentVariable("ASPNETCORE_TRAKT_CLIENT_SECRET");
        _baseUri = Environment.GetEnvironmentVariable("ASPNETCORE_BASE_URL");
    }

    public async Task<SignInResponse> SignIn(SignInRequest request)
    {
        var authorizeResponse = await GetToken(request.Code);

        if (authorizeResponse == null)
        {
            return new SignInResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "No response received from Trakt",
                }
            };
        }

        if (authorizeResponse.Error != null)
        {
            return new SignInResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = $"Received error from Trakt: '{authorizeResponse.ErrorDescription}'.",
                }
            };
        }

        var profile = await GetSettings(authorizeResponse.AccessToken);

        var user = new User
        {
            Identifier = Guid.NewGuid(),
            Username = profile.User.Username,
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
            ThirdPartyIntegrations = new List<ThirdPartyIntegration>()
        }; 

        if (request.UserIdentifier.HasValue)
        {
            var userResponse = await _usersService.GetUserByReference(request.UserIdentifier!.Value);

            if (userResponse.HasError)
            {
                return new SignInResponse
                {
                    HasError = true,
                    Error = userResponse.Error
                };
            }
            
            user = userResponse.User;
        }
        else
        {
            var userResponse = await _usersService.GetUserByUsername(user.Username);

            if (userResponse == null)
            {
                var userRecord = await _usersService.CreateUser(UserMapper.MapRecord(user));

                if (userRecord == null)
                {
                    return new SignInResponse
                    {
                        HasError = true,
                        Error = new Error
                        {
                            UserMessage = "Failed to create a user",
                        }
                    };
                }
                
                user = UserMapper.Map(userRecord);
            }
            else
            {
                user = UserMapper.Map(userResponse);
            }
        }
        
        var thirdPartyIntegration = new ThirdPartyIntegration
        {
            Identifier = Guid.NewGuid(),
            Provider = Provider.Trakt,
            Token = authorizeResponse.AccessToken,
            RefreshToken = authorizeResponse.RefreshToken,
            ExpiresAt = DateTime.Now.AddSeconds(authorizeResponse.ExpiresInSeconds)
        };

        if (user.ThirdPartyIntegrations.Any(x => x.Provider == Provider.Trakt))
        {
            foreach (var userThirdPartyIntegration in user.ThirdPartyIntegrations)
            {
                if (userThirdPartyIntegration.Provider == Provider.Trakt)
                {
                    userThirdPartyIntegration.Identifier = Guid.NewGuid();
                    userThirdPartyIntegration.Token = authorizeResponse.AccessToken;
                    userThirdPartyIntegration.RefreshToken = authorizeResponse.RefreshToken;
                    userThirdPartyIntegration.ExpiresAt = DateTime.Now.AddSeconds(authorizeResponse.ExpiresInSeconds);
                }
            }
        }
        else
        {
            user.ThirdPartyIntegrations.Add(thirdPartyIntegration);
        }

        await _usersService.UpdateUser(user);

        var session = await _sessionService.GetSessionByUserIdentifier(user.Identifier);

        if (session == null)
        {
            session = await _sessionService.CreateSession(user.Identifier, request.Remember);
            _sessionFactory.AddSession(session.Reference(), session);
        }

        return new SignInResponse
        {
            SessionId = session.Reference(),
        };
    }

    public async Task<TraktAuthResponse?> GetToken(string code)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.Add("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.Add("trakt-api-key", _clientId);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Trackster/1.0 (+https://trackster.miloszdura.com/)");

            var body = new
            {
                code = code,
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

                    Console.WriteLine($"[DEBUG] - Received response from Trakt Auth {responseData}.");

                    var parsedData = JsonConvert.DeserializeObject<TraktAuthResponse>(responseData);
                    return parsedData;
                }
            }
        }
    }

    public Task<RegisterResponse> Register(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<TraktProfileResponse?> GetProfile(string username, string accessToken)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.Add("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.Add("trakt-api-key", _clientId);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Trackster/1.0 (+https://trackster.miloszdura.com/)");

            using (var response = await httpClient.GetAsync($"users/{username}"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<TraktProfileResponse>(responseData);
                return parsedData;
            }
        }
    }

    public async Task<TraktSettingsResponse?> GetSettings(string accessToken)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.Add("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.Add("trakt-api-key", _clientId);
            httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {accessToken}");
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Trackster/1.0 (+https://trackster.miloszdura.com/)");

            using (var response = await httpClient.GetAsync($"users/settings"))
            {
                string responseData = await response.Content.ReadAsStringAsync();
                var parsedData = JsonConvert.DeserializeObject<TraktSettingsResponse>(responseData);
                return parsedData;
            }
        }
    }

    public async Task<TraktRefreshTokenResponse> RefreshToken(RefreshTokenRequest request)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.Add("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.Add("trakt-api-key", _clientId);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Trackster/1.0 (+https://trackster.miloszdura.com/)");

            var body = new
            {
                refresh_token = request.Token,
                client_id = _clientId,
                client_secret = _clientSecret,
                redirect_uri = $"{_baseUri}/authorize/trakt",
                grant_type = "refresh_token",
            };

            using (var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.Default, "application/json"))
            {
                using (var response = await httpClient.PostAsync("oauth/token", content))
                {
                    string responseData = await response.Content.ReadAsStringAsync();
                    var parsedData = JsonConvert.DeserializeObject<TraktRefreshTokenResponse>(responseData);
                    return parsedData;
                }
            }
        }
    }

    public async void SignOut(SignOutRequest request)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.Add("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.Add("trakt-api-key", _clientId);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Trackster/1.0 (+https://trackster.miloszdura.com/)");

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