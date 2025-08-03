using Trackster.Api.Core.Helpers;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Movies.Types;

namespace Trackster.Api.Features.Movies;

public interface IMoviesService
{
    GetAllMoviesResponse GetAllWatchedMovies(string username, int results, int page);
    GetMovieResponse GetMovieBySlug(string slug);
    Task<MovieRecord> SearchForMovieBy(string title, int year);
    Task ImportMovie(UserRecord user, MovieRecord movie);

    GetMovieWatchedHistoryResponse GetWatchedHistoryBySlug(string username, string slug);
    MovieRecord? GetMovieByTmdbId(string tmdbId);
    Task MarkMovieAsWatched(string username, string tmdbId, DateTime watchedAt);
}

public class MoviesService : IMoviesService
{
    private readonly IMoviesRepository _repository;
    private readonly TmdbImportProvider _detailsProvider;

    public MoviesService(IMoviesRepository repository)
    {
        _repository = repository;
        _detailsProvider = new TmdbImportProvider();
    }
    
    public GetAllMoviesResponse GetAllWatchedMovies(string username, int results, int page)
    {
        var movies = _repository.GetAllWatchedMovies(username, results, page);

        return new GetAllMoviesResponse
        {
            WatchedMovies = movies
        };
    }
    
    public GetMovieResponse GetMovieBySlug(string slug)
    {
        var movie = _repository.GetMovieBySlug(slug);

        if (movie != null)
        {
            return new GetMovieResponse
            {
                Movie = new Movie
                {
                    Identifier = movie.Identifier,
                    Slug = movie.Slug,
                    Title = movie.Title,
                    Year = movie.Year,
                    Overview = movie.Overview,
                    Poster = movie.Poster,
                    TMDB = movie.TMDB
                }
            };
        }
        
        return new GetMovieResponse();
    }

    public async Task<MovieRecord> SearchForMovieBy(string title, int year)
    {
        var searchResults = await _detailsProvider.FindMovieByTitleAndYear(title, year);
        var tmdbReference = searchResults.Results.FirstOrDefault()?.Id.ToString();
        var movie = await _detailsProvider.GetDetailsForMovie(tmdbReference ?? "");
        
        return new MovieRecord
        {
            Identifier = Guid.NewGuid(),
            Title = title,
            Slug = SlugHelper.GenerateSlugFor(title),
            TMDB = tmdbReference,
            Year = year,
            Overview = movie.Overview,
            Poster = $"https://image.tmdb.org/t/p/w300{movie.PosterUrl}"
        };
    }

    public async Task ImportMovie(UserRecord user, MovieRecord movie)
    {
        await _repository.ImportMovie(user, movie);
    }
    
    public GetMovieWatchedHistoryResponse GetWatchedHistoryBySlug(string username, string slug)
    {
        var movieWatchHistory = _repository.GetWatchedHistoryBySlug(username, slug);

        if (movieWatchHistory != null)
        {
            return new GetMovieWatchedHistoryResponse
            {
                WatchHistory = movieWatchHistory.ConvertAll((episode) =>
                {
                    return new WatchedMovie
                    {
                        WatchedAt = episode.WatchedAt
                    };
                })
            };
        }
        
        return new GetMovieWatchedHistoryResponse();
    }

    public MovieRecord? GetMovieByTmdbId(string tmdbId)
    {
        return _repository.GetMovieByTmdbId(tmdbId);
    }

    public async Task MarkMovieAsWatched(string username, string tmdbId, DateTime watchedAt)
    {
        await _repository.MarkMovieAsWatched(username, tmdbId, watchedAt);
    }
}