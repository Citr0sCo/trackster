using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Spotify.Types;

public class SpotifyTokenResponse
{
    [JsonProperty("access_token")] 
    public string AccessToken { get; set; }
    
    [JsonProperty("refresh_token")] 
    public string RefreshToken { get; set; }
}