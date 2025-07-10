using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Plex.Types;

public class PlexActivityResponse
{
    [JsonProperty("response")]
    public PlexResponse? Response { get; set; }
}

public class PlexResponse
{
    [JsonProperty("data")]
    public PlexData? Data { get; set; }
}

public class PlexData
{
    public PlexData()
    {
        Sessions = new List<PlexSession>();
    }
        
    [JsonProperty("sessions")]
    public List<PlexSession> Sessions { get; set; }
}

public class PlexSession
{
    [JsonProperty("user")]
    public string? User { get; set; }
        
    [JsonProperty("full_title")]
    public string? FullTitle { get; set; }
        
    [JsonProperty("state")]
    public string? State { get; set; }
        
    [JsonProperty("progress_percent")]
    public int ProgressPercentage { get; set; }
        
    [JsonProperty("view_offset")]
    public int? ViewOffset { get; set; }
        
    [JsonProperty("duration")]
    public int? Duration { get; set; }
        
    [JsonProperty("video_decision")]
    public string? VideoDecision { get; set; }
}