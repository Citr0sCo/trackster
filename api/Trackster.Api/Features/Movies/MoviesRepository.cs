using Microsoft.EntityFrameworkCore;
using Trackster.Api.Core.Helpers;
using Trackster.Api.Data;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Movies.Types;

namespace Trackster.Api.Features.Movies;

public interface IMoviesRepository
{
    Task SaveMovie(MovieRecord movie);
    Task ImportMovie(UserRecord user, MovieRecord movie, List<GenreRecord> genres);
    List<WatchedMovie> GetAllWatchedMovies(string username, int results, int page);
    Movie? GetMovieBySlug(string slug);
    List<WatchedMovie> GetWatchedHistoryBySlug(string username, string slug);
    MovieRecord? GetMovieByTmdbId(string tmdbId);
    Task MarkMovieAsWatched(UserRecord user, MovieRecord movie, DateTime watchedAt);
    MovieUserRecord? GetWatchedMovieByLastWatchedAt(string username, string tmdbId, DateTime watchedAt);
    Task<MovieRecord> UpdateMovie(MovieRecord movieRecord, List<GenreRecord> genres);
    Task<List<GenreRecord>> FindOrCreateGenres(List<string> genres);
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
                    
                    var movie = context.Movies.FirstOrDefault(x => x.Identifier.ToString().ToUpper() == watchedMovie.Movie.Identifier.ToString().ToUpper());
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
    
    public Movie? GetMovieBySlug(string slug)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                var movie = context.Movies
                    .FirstOrDefault(x => x.Slug.ToUpper() == slug.ToUpper());

                if (movie == null)
                    return null;

                var genres = context.MovieGenres
                    .Include(x => x.Genre)
                    .Include(x => x.Movie)
                    .Where(x => x.Movie.Identifier == movie.Identifier)
                    .ToList();

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
                        Identifier = x.Genre.Identifier,
                        Name = x.Genre.Name
                    })
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

    public async Task MarkMovieAsWatched(UserRecord user, MovieRecord movie, DateTime watchedAt)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var existingUser = context.Users.FirstOrDefault(x => x.Username.ToUpper() == user.Username.ToUpper());

                if (existingUser == null)
                {
                    Console.WriteLine($"[ERROR] - User '{user.Username}' doesn't exist. Creating...");
                    
                    existingUser = user;
                    context.Users.Add(existingUser);
                }

                var existingMovie = context.Movies.FirstOrDefault(x => x.TMDB == movie.TMDB);

                if (existingMovie == null)
                {
                    Console.WriteLine($"[INFO] - Movie '{movie.Title}' doesn't exist. Creating...");
                    
                    existingMovie = movie;
                    context.Movies.Add(existingMovie);
                }
                
                var existingMovieUserRecord = context.MovieUserLinks.FirstOrDefault(x =>
                    x.User.Username.ToUpper() == user.Username.ToUpper() &&
                    x.Movie.TMDB.ToUpper() == movie.TMDB.ToUpper() &&
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

                    Console.WriteLine($"[INFO] - Movie-User Link '{user.Username}'-'{existingMovie.Title}' doesn't exist. Creating...");
                    
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

    public async Task<MovieRecord> UpdateMovie(MovieRecord movieRecord, List<GenreRecord> genres)
    {
        await using (var context = new DatabaseContext())
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var existingMovie = context.Movies.FirstOrDefault(x => x.Identifier.ToString().ToUpper() == movieRecord.Identifier.ToString().ToUpper());

                if (existingMovie == null)
                    return movieRecord;
                
                existingMovie.Title = movieRecord.Title;

                foreach (var genre in genres)
                {
                    var existingGenre = context.Genres.FirstOrDefault(x => x.Identifier == genre.Identifier);

                    if (existingGenre == null)
                        continue;
                    
                    var existingMovieGenre = context.MovieGenres.FirstOrDefault(x => x.Movie.Identifier == movieRecord.Identifier && x.Genre.Identifier == genre.Identifier);

                    if (existingMovieGenre != null)
                        continue;

                    var movieGenreRecord = new MovieGenreRecord
                    {
                        Identifier = Guid.NewGuid(),
                        Movie = existingMovie,
                        Genre = existingGenre,
                    };
                    
                    context.MovieGenres.Add(movieGenreRecord);
                }
                
                context.Movies.Update(existingMovie);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return existingMovie;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                await transaction.RollbackAsync();
            }

            return movieRecord;
        }
    }

    public async Task<List<GenreRecord>> FindOrCreateGenres(List<string> genres)
    {
        await using (var context = new DatabaseContext())
        await using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var genreRecords = new List<GenreRecord>();
                
                foreach (var genre in genres)
                {
                    var existingGenre = context.Genres.FirstOrDefault(x => x.Name.ToUpper() == genre.ToUpper());

                    if (existingGenre == null)
                    {
                        existingGenre = new GenreRecord
                        {
                            Identifier = Guid.NewGuid(),
                            Name = genre,
                        };
                        
                        context.Genres.Add(existingGenre);
                    }
                    
                    genreRecords.Add(existingGenre);
                }
                
                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return genreRecords;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                await transaction.RollbackAsync();
            }

            return new List<GenreRecord>();
        }
    }

    public async Task ImportMovie(UserRecord user, MovieRecord movie,  List<GenreRecord> genres)
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
                
                foreach (var genre in genres)
                {
                    var existingGenre = context.Genres.FirstOrDefault(x => x.Identifier == genre.Identifier);

                    if (existingGenre == null)
                    {    
                        Console.WriteLine($"[INFO] - Genre '{genre.Name}' doesn't exist. Creating...");
                        context.Add(genre);
                    }
                    
                    context.MovieGenres.Add(new MovieGenreRecord
                    {
                        Identifier = Guid.NewGuid(),
                        Movie = movie,
                        Genre = genre,
                    });
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

    public async Task SaveMovie(MovieRecord movie)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
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