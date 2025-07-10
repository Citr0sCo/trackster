using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.PiHole.Types;

public class PiHoleAuthenticateRequest
{
    [JsonProperty("password")]
    public string Password { get; set; }
}