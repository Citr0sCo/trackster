using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Sonarr.Types;

public class SonarrMissing
{
    [JsonProperty("totalRecords")]
    public int Total { get; set; }
}