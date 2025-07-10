using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Sonarr.Types;

public class SonarrSeriesStatistics
{
    [JsonProperty("sizeOnDisk")]
    public long SizeOnDisk { get; set; }
}