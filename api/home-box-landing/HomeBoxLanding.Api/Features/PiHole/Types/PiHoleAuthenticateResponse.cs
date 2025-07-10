using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.PiHole.Types;

public class PiHoleAuthenticateResponse
{
    [JsonProperty("session")]
    public PiHoleAuthenticateSession Session { get; set; }

    public class PiHoleAuthenticateSession
    {
        [JsonProperty("sid")]
        public string SessionId { get; set; }
    }
}