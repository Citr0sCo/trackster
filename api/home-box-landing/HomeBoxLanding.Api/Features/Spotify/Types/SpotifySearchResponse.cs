using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Spotify.Types;

public class SpotifySearchResponse
{
    [JsonProperty("tracks")] 
    public SpotifySearchTracks Tracks { get; set; }
}

public class SpotifySearchTracks
{
    public SpotifySearchTracks()
    {
        Items = new List<SpotifySearchTrackItem>();
    }
    
    [JsonProperty("items")] 
    public List<SpotifySearchTrackItem> Items { get; set; }
}

public class SpotifySearchTrackItem
{
    [JsonProperty("uri")] 
    public string SpotifyUri { get; set; }
    
    [JsonProperty("name")] 
    public string Name { get; set; }
    
    [JsonProperty("artists")] 
    public List<SpotifySearchTrackItemArtist> Artists { get; set; }
}

public class SpotifySearchTrackItemArtist
{
    [JsonProperty("name")] 
    public string Name { get; set; }
}