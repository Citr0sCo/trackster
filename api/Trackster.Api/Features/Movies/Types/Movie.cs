namespace Trackster.Api.Features.Movies.Types;

public class Movie
{
    public Movie()
    {
        Genres = new List<Genre>();
    }
    
    public Guid Identifier { get; set; }
    public string Title { get; set; }
    public int Year { get; set; }
    public string TMDB { get; set; }
    public string? Poster { get; set; }
    public string? Overview { get; set; }
    public string Slug { get; set; }
    public List<Genre> Genres { get; set; }
}