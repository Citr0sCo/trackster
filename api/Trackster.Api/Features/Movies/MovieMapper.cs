using Trackster.Api.Data.Records;
using Trackster.Api.Features.Movies.Types;

namespace Trackster.Api.Features.Movies;

public static class MovieMapper
{
    public static Movie Map(MovieRecord movie, List<GenreRecord> genres)
    {
        return new Movie
        {
            Identifier = movie.Identifier,
            Title = movie.Title,
            Year = movie.Year,
            TMDB =  movie.TMDB,
            Poster = movie.Poster,
            Overview = movie.Overview,
            Slug = movie.Slug,
            Genres = genres.ConvertAll(x => new Genre
            {
                Identifier = x.Identifier,
                Name = x.Name
            })
        };
    }
}