namespace Trackster.Api.Features.Shows.Types;

public class WatchedShow
{
    public Show Show { get; set; }
    public Season Season { get; set; }
    public Episode Episode { get; set; }
    public DateTime WatchedAt { get; set; }
}