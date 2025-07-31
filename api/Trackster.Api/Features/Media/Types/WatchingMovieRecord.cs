using Trackster.Api.Data.Records;

namespace Trackster.Api.Features.Media.Types;

public class WatchingMovieRecord
{
    public WatchingAction Action { get; set; }
    public MovieRecord Movie { get; set; }
    public DateTime StartedAt { get; set; }
    public int MillisecondsWatched { get; set; }
}