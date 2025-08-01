using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Media.Importers.TraktImporter.Types;
using Trackster.Api.Features.Movies.Types;

namespace Trackster.Api.Features.Movies;

public interface IMoviesService
{
    GetAllMoviesResponse GetAllWatchedMovies(string username);
    GetMovieResponse GetMovieBySlug(string slug);
    Task<MovieRecord> SearchForMovieBy(string title, int year);
    Task ImportMovies(string username, List<TraktMovieResponse> movies);
    void ImportMovie(string username, MovieRecord movie);

    GetMovieWatchedHistoryResponse GetWatchedHistoryBySlug(string username, string slug);
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
    
    public GetAllMoviesResponse GetAllWatchedMovies(string username)
    {
        var movies = _repository.GetAllWatchedMovies(username);

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
            TMDB = tmdbReference,
            Year = year,
            Overview = movie.Overview,
            Poster = $"https://image.tmdb.org/t/p/w185{movie.PosterUrl}"
        };
    }

    public Task ImportMovies(string username, List<TraktMovieResponse> movies)
    {
        return _repository.ImportMovies(username, movies);
    }

    public void ImportMovie(string username, MovieRecord movie)
    {
        _repository.ImportMovie(username, movie);
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
}