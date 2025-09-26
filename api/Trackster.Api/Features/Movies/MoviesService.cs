using Trackster.Api.Core.Helpers;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Movies.Types;

namespace Trackster.Api.Features.Movies;

public interface IMoviesService
{
    GetAllMoviesResponse GetAllWatchedMovies(string username, int results, int page);
    GetMovieResponse GetMovieBySlug(string slug);
    Task<MovieRecord> SearchForMovieBy(string title, int year, bool requestDebug);
    Task ImportMovie(UserRecord user, MovieRecord movie, List<GenreRecord> genres);

    GetMovieWatchedHistoryResponse GetWatchedHistoryBySlug(string username, string slug);
    MovieRecord? GetMovieByTmdbId(string tmdbId);
    Task MarkMovieAsWatched(UserRecord user, MovieRecord movie, DateTime watchedAt);
    MovieUserRecord? GetWatchedMovieByLastWatchedAt(string username, string tmdbId, DateTime watchedAt);
    Task<List<GenreRecord>> FindOrCreateGenres(List<string> genres);
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
                Movie = movie
            };
        }
        
        return new GetMovieResponse();
    }

    public async Task<MovieRecord> SearchForMovieBy(string title, int year, bool requestDebug)
    {
        var searchResults = await _detailsProvider.FindMovieByTitleAndYear(title, year, requestDebug);
        var tmdbReference = searchResults.Results.FirstOrDefault()?.Id.ToString();
        var details = await _detailsProvider.GetDetailsForMovie(tmdbReference ?? "", requestDebug);

        var movie = new MovieRecord
        {
            Identifier = Guid.NewGuid(),
            Title = title,
            Slug = SlugHelper.GenerateSlugFor(title),
            TMDB = tmdbReference,
            Year = year,
            Overview = details.Overview,
            Poster = $"https://image.tmdb.org/t/p/w300{details.PosterUrl}"
        };

        await _repository.SaveMovie(movie);
        
        return movie;
    }

    public async Task ImportMovie(UserRecord user, MovieRecord movie, List<GenreRecord> genres)
    {
        await _repository.ImportMovie(user, movie, genres);
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

    public async Task MarkMovieAsWatched(UserRecord user, MovieRecord movie, DateTime watchedAt)
    {
        await _repository.MarkMovieAsWatched(user, movie, watchedAt);
    }

    public MovieUserRecord? GetWatchedMovieByLastWatchedAt(string username, string tmdbId, DateTime watchedAt)
    {
       return _repository.GetWatchedMovieByLastWatchedAt(username, tmdbId, watchedAt);
    }

    public async Task<List<GenreRecord>> FindOrCreateGenres(List<string> genres)
    {
        return await _repository.FindOrCreateGenres(genres);
    }

    public async Task<GetMovieResponse> ImportDataForMovie(string slug)
    {
        var movie = _repository.GetMovieBySlug(slug);

        if (movie != null)
        {
            var details = await _detailsProvider.GetDetailsForMovie(movie.TMDB);

            var genres = await _repository.FindOrCreateGenres(details.Genres.ConvertAll((genre) => genre.Name));
            
            var updatedMovie = await _repository.UpdateMovie(new MovieRecord
            {
                Identifier = movie.Identifier,
                Title = details.Title
            }, genres);

            return new GetMovieResponse
            {
                Movie = MovieMapper.Map(updatedMovie, genres)
            };
        }
        
        return new GetMovieResponse();
    }
}