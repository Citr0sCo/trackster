using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Sonarr.Types;

public class SonarrSeries
{
    [JsonProperty("id")]
    public int Identifier { get; set; }
    
    [JsonProperty("title")]
    public string Title { get; set; }
    
    [JsonProperty("statistics")]
    public SonarrSeriesStatistics Statistics { get; set; }
}