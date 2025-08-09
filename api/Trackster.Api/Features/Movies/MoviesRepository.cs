using Trackster.Api.Core.Helpers;
using Trackster.Api.Data;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Movies.Types;

namespace Trackster.Api.Features.Movies;

public interface IMoviesRepository
{
    Task ImportMovie(UserRecord user, MovieRecord movie);
    List<WatchedMovie> GetAllWatchedMovies(string username, int results, int page);
    Movie? GetMovieBySlug(string slug);
    List<WatchedMovie> GetWatchedHistoryBySlug(string username, string slug);
    MovieRecord? GetMovieByTmdbId(string tmdbId);
    Task MarkMovieAsWatched(string username, string tmdbId, DateTime watchedAt);
    MovieUserRecord? GetWatchedMovieByLastWatchedAt(string username, string tmdbId, DateTime watchedAt);
}

public class MoviesRepository : IMoviesRepository
{
    public List<WatchedMovie> GetAllWatchedMovies(string username, int results, int page)
    {
        using (var context = new DatabaseContext())
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                var watchedMovies = context.MovieUserLinks
                    .Where(x => x.User.Username.ToUpper() == username.ToUpper())
                    .OrderByDescending(x => x.WatchedAt)
                    .Skip((page - 1) * results)
                    .Take(results)
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

    public MovieRecord? GetMovieByTmdbId(string tmdbId)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                return context.Movies
                    .FirstOrDefault(x => x.TMDB.ToUpper() == tmdbId.ToUpper());
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }

    public async Task MarkMovieAsWatched(string username, string tmdbId, DateTime watchedAt)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var existingUser = context.Users.FirstOrDefault(x => x.Username.ToUpper() == username.ToUpper());

                if (existingUser == null)
                {
                    Console.WriteLine($"[ERROR] - User '{username}' doesn't exist.");
                    return;
                }

                var existingMovie = context.Movies.FirstOrDefault(x => x.TMDB == tmdbId);

                if (existingMovie == null)
                {
                    Console.WriteLine($"[INFO] - Movie '{tmdbId}' doesn't exist.");
                    return;
                }
                
                var existingMovieUserRecord = context.MovieUserLinks.FirstOrDefault(x =>
                    x.User.Username.ToUpper() == username.ToUpper() &&
                    x.Movie.TMDB.ToUpper() == tmdbId.ToUpper() &&
                    x.WatchedAt >= watchedAt.AddHours(-1) &&
                    x.WatchedAt <= watchedAt.AddHours(1)
                );

                if (existingMovieUserRecord == null)
                {
                    var movieUserRecord = new MovieUserRecord
                    {
                        Identifier = Guid.NewGuid(),
                        User = existingUser,
                        Movie = existingMovie,
                        WatchedAt = watchedAt,
                    };

                    Console.WriteLine($"[INFO] - Movie-User Link '{username}'-'{existingMovie.Title}' doesn't exist. Creating...");
                    context.Add(movieUserRecord);
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

    public MovieUserRecord? GetWatchedMovieByLastWatchedAt(string username, string tmdbId, DateTime watchedAt)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                var existingUser = context.Users.FirstOrDefault(x => x.Username.ToUpper() == username.ToUpper());

                if (existingUser == null)
                {
                    Console.WriteLine($"[ERROR] - User '{username}' doesn't exist.");
                    return null;
                }

                var existingMovie = context.Movies.FirstOrDefault(x => x.TMDB == tmdbId);

                if (existingMovie == null)
                {
                    Console.WriteLine($"[INFO] - Movie '{tmdbId}' doesn't exist.");
                    return null;
                }
                
                var existingMovieUserRecord = context.MovieUserLinks.FirstOrDefault(x =>
                    x.User.Username.ToUpper() == username.ToUpper() &&
                    x.Movie.TMDB.ToUpper() == tmdbId.ToUpper() &&
                    x.WatchedAt >= watchedAt.AddHours(-1) &&
                    x.WatchedAt <= watchedAt.AddHours(1)
                );

                return existingMovieUserRecord;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        return null;
    }

    public async Task ImportMovie(UserRecord user, MovieRecord movie)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var existingUser = context.Users.FirstOrDefault(x => x.Username.ToUpper() == user.Username.ToUpper());

                if (existingUser == null)
                {
                    Console.WriteLine($"[INFO] - User '{user.Username}' doesn't exist. Creating...");
                    context.Add(user);
                }

                var existingMovie = context.Movies.FirstOrDefault(x => x.TMDB == movie.TMDB);

                if (existingMovie == null)
                {    
                    Console.WriteLine($"[INFO] - Movie '{movie.Title}' doesn't exist. Creating...");
                    context.Add(movie);
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
}