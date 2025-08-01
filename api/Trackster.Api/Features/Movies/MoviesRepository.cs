using Trackster.Api.Core.Helpers;
using Trackster.Api.Data;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Media.Importers.TraktImporter.Types;
using Trackster.Api.Features.Movies.Types;

namespace Trackster.Api.Features.Movies;

public interface IMoviesRepository
{
    Task ImportMovies(string username, List<TraktMovieResponse> movies);
    void ImportMovie(string username, MovieRecord movie);
    List<WatchedMovie> GetAllWatchedMovies(string username);
    Movie? GetMovieBySlug(string slug);
    List<WatchedMovie> GetWatchedHistoryBySlug(string username, string slug);
}

public class MoviesRepository : IMoviesRepository
{
    private readonly TmdbImportProvider _detailsProvider;

    public MoviesRepository()
    {
        _detailsProvider = new TmdbImportProvider();
    }
    
    public async Task ImportMovies(string username, List<TraktMovieResponse> movies)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var existingUser = context.Users.FirstOrDefault(x => x.Username.ToUpper() == username.ToUpper());

                if (existingUser == null)
                {
                    existingUser = new UserRecord
                    {
                        Identifier = Guid.NewGuid(),
                        Username = username
                    };

                    Console.WriteLine($"[INFO] - User '{username}' doesn't exist. Creating...");
                    context.Add(existingUser);
                }

                var moviesProcessed = 0;
                foreach (var movie in movies)
                {
                    var existingMovie = context.Movies.FirstOrDefault(x => x.TMDB == movie.Movie.Ids.TMDB);

                    if (existingMovie == null)
                    {
                        var details = await _detailsProvider.GetDetailsForMovie(movie.Movie.Ids.TMDB);
                        
                        existingMovie = new MovieRecord
                        {
                            Identifier = Guid.NewGuid(),
                            Title = movie.Movie.Title,
                            Slug = SlugHelper.GenerateSlugFor(movie.Movie.Title),
                            Year = movie.Movie.Year,
                            TMDB = movie.Movie.Ids.TMDB,
                            Poster = $"https://image.tmdb.org/t/p/w185{details.PosterUrl}",
                            Overview = details?.Overview,
                        };
                        
                        Console.WriteLine($"[INFO] - Movie '{movie.Movie.Title}' doesn't exist. Creating...");
                        context.Add(existingMovie);
                    }

                    var existingMovieUserRecord = context.MovieUserLinks.FirstOrDefault(x =>
                        x.User.Username.ToUpper() == username.ToUpper() &&
                        x.Movie.TMDB == movie.Movie.Ids.TMDB
                    );
                    
                    if ((existingMovieUserRecord?.WatchedAt - movie.LastWatchedAt)?.TotalHours > 1)
                        existingMovieUserRecord = null;

                    if (existingMovieUserRecord == null)
                    {
                        var movieUserRecord = new MovieUserRecord
                        {
                            Identifier = Guid.NewGuid(),
                            User = existingUser,
                            Movie = existingMovie,
                            WatchedAt = movie.LastWatchedAt
                        };

                        Console.WriteLine($"[INFO] - Movie-User Link '{username}'-'{movie.Movie.Title}' doesn't exist. Creating...");
                        context.Add(movieUserRecord);
                    }

                    moviesProcessed++;
                    Console.WriteLine($"[INFO] - Movie {moviesProcessed}/{movies.Count} processed.");
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                await transaction.RollbackAsync();
            }
        }
    }
    
    public List<WatchedMovie> GetAllWatchedMovies(string username)
    {
        using (var context = new DatabaseContext())
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                var watchedMovies = context.MovieUserLinks
                    .Where(x => x.User.Username.ToUpper() == username.ToUpper())
                    .OrderByDescending(x => x.WatchedAt)
                    .Select(x => new WatchedMovie
                    {
                        Movie = new Movie
                        {
                            Identifier = x.Movie.Identifier,
                            Title = x.Movie.Title,
                            Slug = x.Movie.Slug,
                            Year = x.Movie.Year,
                            TMDB =  x.Movie.TMDB,
                            Poster = x.Movie.Poster,
                            Overview = x.Movie.Overview
                        },
                        WatchedAt = x.WatchedAt
                    })
                    .ToList();

                foreach (var watchedMovie in watchedMovies)
                {
                    if (!string.IsNullOrEmpty(watchedMovie.Movie.Slug))
                        continue;
                    
                    watchedMovie.Movie.Slug = SlugHelper.GenerateSlugFor(watchedMovie.Movie.Title);
                    
                    var movie = context.Movies.FirstOrDefault(x => x.Identifier == watchedMovie.Movie.Identifier);
                    movie.Slug = watchedMovie.Movie.Slug;
                    context.Update(movie);
                }

                context.SaveChanges();
                transaction.Commit();
                
                return watchedMovies;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                transaction.Rollback();
            }
        }

        return new List<WatchedMovie>();
    }
    
    public Movie GetMovieBySlug(string slug)
    {
        
        using (var context = new DatabaseContext())
        {
            try
            {
                var movie = context.Movies
                    .FirstOrDefault(x => x.Slug.ToUpper() == slug.ToUpper());

                if (movie == null)
                    return null;

                return new Movie
                {
                    Identifier = movie.Identifier,
                    Title = movie.Title,
                    Year = movie.Year,
                    TMDB =  movie.TMDB,
                    Poster = movie.Poster,
                    Overview = movie.Overview,
                    Slug = movie.Slug,
                };
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }

    public List<WatchedMovie> GetWatchedHistoryBySlug(string username, string slug)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                var watchHistory = context.MovieUserLinks
                    .Where(x => x.User.Username.ToUpper() == username.ToUpper())
                    .Where(x => x.Movie.Slug.ToUpper() == slug.ToUpper())
                    .ToList();

                if (watchHistory == null)
                    return null;

                return watchHistory.ConvertAll((episode) =>
                {
                    return new WatchedMovie
                    {
                        WatchedAt = episode.WatchedAt
                    };
                });
            }
            catch (Exception exception)
            {
                return new  List<WatchedMovie>();
            }
        }
    }

    public void ImportMovie(string username, MovieRecord movie)
    {
        using (var context = new DatabaseContext())
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                var existingUser = context.Users.FirstOrDefault(x => x.Username.ToUpper() == username.ToUpper());

                if (existingUser == null)
                {
                    existingUser = new UserRecord
                    {
                        Identifier = Guid.NewGuid(),
                        Username = username
                    };

                    context.Add(existingUser);
                }

                var existingMovie = context.Movies.FirstOrDefault(x => x.TMDB == movie.TMDB);

                if (existingMovie == null)
                {
                    existingMovie = movie;
                    context.Add(existingMovie);
                }

                var existingMovieUserRecord = context.MovieUserLinks.FirstOrDefault(x =>
                    x.User.Username.ToUpper() == username.ToUpper() &&
                    x.Movie.TMDB == movie.TMDB
                );

                if (existingMovieUserRecord == null)
                {
                    var movieUserRecord = new MovieUserRecord
                    {
                        Identifier = Guid.NewGuid(),
                        User = existingUser,
                        Movie = existingMovie,
                        WatchedAt = DateTime.Now
                    };

                    context.Add(movieUserRecord);
                }

                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                transaction.Rollback();
            }
        }
    }
}