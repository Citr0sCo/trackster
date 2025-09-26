namespace Trackster.Api.Features.Shows.Types;

public class WatchedEpisode
{
    public Episode Episode { get; set; }
    public DateTime WatchedAt { get; set; }
}