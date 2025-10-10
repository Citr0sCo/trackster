using Newtonsoft.Json;
using Trackster.Api.Core.Types;
using Trackster.Api.Data.Records;
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

        if (Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") == "Development")
            _baseUri = "http://localhost:4200";
    }

    public async Task<SignInResponse> SignIn(SignInRequest request)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.Add("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.Add("trakt-api-key", _clientId);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Trackster/1.0 (+https://trackster.miloszdura.com/)");

            var body = new
            {
                code = request.Code,
                client_id = _clientId,
                client_secret = _clientSecret,
                redirect_uri = $"{_baseUri}/app/authorize/trakt",
                grant_type = "authorization_code",
            };

            using (var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.Default,
                       "application/json"))
            {
                using (var response = await httpClient.PostAsync("oauth/token", content))
                {
                    var responseData = await response.Content.ReadAsStringAsync();

                    Console.WriteLine($"[DEBUG] - Received response from Trakt Auth {responseData}.");

                    var parsedData = JsonConvert.DeserializeObject<TraktAuthResponse>(responseData);

                    if (parsedData == null)
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

                    if (parsedData.Error != null)
                    {
                        return new SignInResponse
                        {
                            HasError = true,
                            Error = new Error
                            {
                                UserMessage = $"Received error from Trakt: '{parsedData.ErrorDescription}'.",
                            }
                        };
                    }

                    var userResponse = await _usersService.GetUserByReference(request.UserIdentifier ?? Guid.Empty);

                    if (userResponse == null)
                    {
                        return new SignInResponse
                        {
                            HasError = true,
                            Error = new Error
                            {
                                UserMessage = "User not found",
                            }
                        };
                    }
                    
                    var thirdPartyIntegration = new ThirdPartyIntegration
                    {
                        Identifier = Guid.NewGuid(),
                        Provider = Provider.Trakt,
                        Token = parsedData.AccessToken,
                        RefreshToken = parsedData.RefreshToken,
                        ExpiresAt = DateTime.Now.AddSeconds(parsedData.ExpiresInSeconds)
                    };

                    if (userResponse.User.ThirdPartyIntegrations.Any(x => x.Provider == Provider.Trakt))
                    {
                        foreach (var userThirdPartyIntegration in userResponse.User.ThirdPartyIntegrations)
                        {
                            if (userThirdPartyIntegration.Provider == Provider.Trakt)
                            {
                                userThirdPartyIntegration.Identifier = Guid.NewGuid();
                                userThirdPartyIntegration.Token = parsedData.AccessToken;
                                userThirdPartyIntegration.RefreshToken = parsedData.RefreshToken;
                                userThirdPartyIntegration.ExpiresAt = DateTime.Now.AddSeconds(parsedData.ExpiresInSeconds);
                            }
                        }
                    }
                    else
                    {
                        userResponse.User.ThirdPartyIntegrations.Add(thirdPartyIntegration);
                    }

                    await _usersService.UpdateUser(userResponse.User);

                    var session = await _sessionService.GetSessionByUserIdentifier(userResponse.User.Identifier);

                    if (session == null)
                    {
                        session = await _sessionService.CreateSession(userResponse.User.Identifier, request.Remember);
                        _sessionFactory.AddSession(session.Reference(), session);
                    }

                    return new SignInResponse
                    {
                        SessionId = session.Reference(),
                    };

                    return new SignInResponse
                    {
                    };
                }
            }
        }

        return new SignInResponse();
    }

    public Task<RegisterResponse> Register(RegisterRequest request)
    {
        throw new NotImplementedException();
    }

    public async Task<TraktProfileResponse?> GetProfile(string username)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.Add("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.Add("trakt-api-key", _clientId);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Trackster/1.0 (+https://trackster.miloszdura.com/)");

            using (var response = await httpClient.GetAsync($"users/{username}"))
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
            httpClient.DefaultRequestHeaders.Add("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.Add("trakt-api-key", _clientId);
            httpClient.DefaultRequestHeaders.Add("User-Agent", "Trackster/1.0 (+https://trackster.miloszdura.com/)");

            var body = new
            {
                token = request.Token,
                client_id = _clientId,
                client_secret = _clientSecret
            };

            using (var content = new StringContent(JsonConvert.SerializeObject(body), System.Text.Encoding.Default,
                       "application/json"))
            {
                using (var response = await httpClient.PostAsync("oauth/revoke", content))
                {
                    await response.Content.ReadAsStringAsync();
                }
            }
        }
    }
}