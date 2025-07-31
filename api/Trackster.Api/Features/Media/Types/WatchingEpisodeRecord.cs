using Trackster.Api.Data.Records;

namespace Trackster.Api.Features.Media.Types;

public class WatchingEpisodeRecord
{
    public EpisodeRecord Episode { get; set; }
    public DateTime StartedAt { get; set; }
    public int MillisecondsWatched { get; set; }
}