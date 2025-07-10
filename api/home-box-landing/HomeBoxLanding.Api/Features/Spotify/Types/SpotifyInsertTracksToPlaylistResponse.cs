using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Spotify.Types;

public class SpotifyInsertTracksToPlaylistResponse
{
    [JsonProperty("snapshot_id")] 
    public string SnaphotId { get; set; }
}