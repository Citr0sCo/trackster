namespace Trackster.Api.Data.Records;

public class WatchingMovieRecord
{
    public string Action { get; set; }
    public MovieRecord Movie { get; set; }
    public int MillisecondsWatched { get; set; }
    public int Duration { get; set; }
    public DateTime LastUpdatedAt { get; set; }
}