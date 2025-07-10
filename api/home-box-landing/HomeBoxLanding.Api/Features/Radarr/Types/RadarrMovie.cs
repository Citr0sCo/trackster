using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Radarr.Types;

public class RadarrMovie
{
    [JsonProperty("id")]
    public int Identifier { get; set; }
    
    [JsonProperty("title")]
    public string Title { get; set; }
    
    [JsonProperty("sizeOnDisk")]
    public long SizeOnDisk { get; set; }
}