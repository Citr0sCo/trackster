namespace Trackster.Api.Features.Webhooks.Types;

public class MarkMediaAsPausedRequest
{
    public Guid UserIdentifier { get; set; }
    public string MediaType { get; set; }
    public int Year { get; set; }
    public string Title { get; set; }
    public string ParentTitle { get; set; }
    public string GrandParentTitle { get; set; }
    public int SeasonNumber { get; set; }
    public int MillisecondsWatched { get; set; }
    public int Duration { get; set; }
    public bool Debug { get; set; }
}