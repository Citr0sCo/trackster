using Trackster.Api.Data.Records;

namespace Trackster.Api.Features.Media.Types;

public class WatchingEpisodeRecord
{
    public string Action { get; set; }
    public EpisodeRecord Episode { get; set; }
    public int MillisecondsWatched { get; set; }
    public int Duration { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}