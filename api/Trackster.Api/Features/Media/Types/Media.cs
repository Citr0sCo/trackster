namespace Trackster.Api.Features.Media.Types;

public class Media
{
    public Guid Identifier { get; set; }
    public string Type { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string TMDB { get; set; }
    public string? Poster { get; set; }
    public string? Overview { get; set; }
    public DateTime WatchedAt { get; set; }
    public string ParentTitle { get; set; }
    public string GrandParentTitle { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string Slug { get; set; }
}