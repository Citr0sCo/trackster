namespace Trackster.Api.Features.Media.Types;

public class Show
{
    public Guid Identifier { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string TMDB { get; set; }
    public string? Poster { get; set; }
    public string? Overview { get; set; }
    public DateTime WatchedAt { get; set; }
}