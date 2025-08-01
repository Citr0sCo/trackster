namespace Trackster.Api.Features.Movies.Types;

public class WatchedMovie
{
    public Movie Movie { get; set; }
    public DateTime WatchedAt { get; set; }
}