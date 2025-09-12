namespace Trackster.Api.Features.Media.Types;

public class MarkMediaAsWatchedRequest
{
    public string Username { get; set; }
    public string MediaType { get; set; }
    public int Year { get; set; }
    public string Title { get; set; }
    public string ParentTitle { get; set; }
    public string GrandParentTitle { get; set; }
    public int ParentIndex { get; set; }
    public bool RequestDebug { get; set; }
    public int SeasonNumber { get; set; }
}