using Trackster.Api.Features.Media.Types;

namespace Trackster.Api.Features.Shows.Types;

public class Show
{
    public Show()
    {
        Genres = new List<Genre>();
    }
    
    public Guid Identifier { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string TMDB { get; set; }
    public string? Poster { get; set; }
    public string? Overview { get; set; }
    public DateTime WatchedAt { get; set; }
    public int SeasonNumber { get; set; }
    public int EpisodeNumber { get; set; }
    public string Slug { get; set; }
    public List<Genre> Genres { get; set; }
}