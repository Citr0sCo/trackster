using Newtonsoft.Json;

namespace Trackster.Api.Features.Authentication.Providers.Trakt;

public class TraktAuthResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
}