using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Radarr.Types;

public class RadarrQueue
{
    [JsonProperty("totalRecords")]
    public int Total { get; set; }
}