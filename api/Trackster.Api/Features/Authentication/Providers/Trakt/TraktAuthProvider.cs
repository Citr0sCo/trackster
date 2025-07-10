using Newtonsoft.Json;
using Trackster.Api.Features.Authentication.Types;

namespace Trackster.Api.Features.Authentication.Providers.Trakt;

public class TraktAuthProvider : IAuthProvider
{
    public bool IsActive { get; } = true;

    public async Task<SignInResponse> SignIn(SignInRequest request)
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient { BaseAddress = baseAddress })
        {
            var body = new
            {
                code = request.Code,
                client_id = "ce40409023d4a567b678e19aa3c4b4dc243d05f85ac624f4d203840227043011",
                client_secret = "c32460422fe12c4943f995bbb1193dd347084fa5557a557dde47fc9dbccf02a4",
                redirect_uri = "http://localhost:4200/authorize/trakt",
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

    public async Task<TraktProfileResponse?> GetProfile()
    {
        var baseAddress = new Uri("https://api.trakt.tv/");

        using (var httpClient = new HttpClient{ BaseAddress = baseAddress })
        {
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-version", "2");
            httpClient.DefaultRequestHeaders.TryAddWithoutValidation("trakt-api-key", "ce40409023d4a567b678e19aa3c4b4dc243d05f85ac624f4d203840227043011");
  
            using(var response = await httpClient.GetAsync("users/citr0s"))
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
                client_id = "ce40409023d4a567b678e19aa3c4b4dc243d05f85ac624f4d203840227043011",
                client_secret = "c32460422fe12c4943f995bbb1193dd347084fa5557a557dde47fc9dbccf02a4"
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