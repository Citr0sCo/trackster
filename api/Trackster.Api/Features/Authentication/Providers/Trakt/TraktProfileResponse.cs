using Newtonsoft.Json;

namespace Trackster.Api.Features.Authentication.Providers.Trakt;

public class TraktProfileResponse
{
    [JsonProperty("id")] 
    public string Identifier { get; set; }
}