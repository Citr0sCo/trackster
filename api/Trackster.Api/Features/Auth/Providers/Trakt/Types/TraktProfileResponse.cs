using Newtonsoft.Json;

namespace Trackster.Api.Features.Auth.Providers.Trakt.Types;

public class TraktProfileResponse
{
    [JsonProperty("username")] 
    public string Username { get; set; }
    
    [JsonProperty("private")] 
    public bool Private { get; set; }
    
    [JsonProperty("deleted")] 
    public bool Deleted { get; set; }
    
    [JsonProperty("name")] 
    public bool Name { get; set; }
    
    [JsonProperty("vip")] 
    public bool Vip { get; set; }
}