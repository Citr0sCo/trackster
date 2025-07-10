using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Sonarr.Types;

public class SonarrQueue
{
    [JsonProperty("totalRecords")]
    public int Total { get; set; }
}