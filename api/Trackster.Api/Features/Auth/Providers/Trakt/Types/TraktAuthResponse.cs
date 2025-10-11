using Newtonsoft.Json;

namespace Trackster.Api.Features.Auth.Providers.Trakt.Types;

public class TraktAuthResponse
{
    [JsonProperty("access_token")]
    public string AccessToken { get; set; }
    
    [JsonProperty("refresh_token")]
    public string RefreshToken { get; set; }
    
    [JsonProperty("expires_in")]
    public int ExpiresInSeconds { get; set; }
    
    [JsonProperty("created_at")]
    public string CreatedAt { get; set; }
    
    [JsonProperty("token_type")]
    public string TokenType { get; set; }
    
    [JsonProperty("error")]
    public string? Error { get; set; }
    
    [JsonProperty("error_description")]
    public string ErrorDescription { get; set; }
}