using Microsoft.EntityFrameworkCore;
using Trackster.Api.Data;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Media.Importers.TraktImporter.Types;
using Trackster.Api.Features.Media.Types;

namespace Trackster.Api.Features.Media;

public interface IMediaRepository
{
    Task ImportMovies(string username, List<TraktMovieResponse> movies);
    Task ImportShows(string username, List<TraktShowResponse> shows);
    List<Movie> GetAllMovies(string username);
    List<Show> GetAllShows(string username);
    void ImportMovie(string username, MovieRecord movie);
    void ImportEpisode(string username, ShowRecord show, SeasonRecord season, EpisodeRecord episode);
    Movie? GetMovieByIdentifier(Guid identifier);
    Show? GetShowByIdentifier(Guid identifier);
}

public class MediaRepository : IMediaRepository
{
    private readonly TmdbImportProvider _detailsProvider;

    public MediaRepository()
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
                            Year = movie.Movie.Year,
                            TMDB = movie.Movie.Ids.TMDB,
                            Poster = $"https://image.tmdb.org/t/p/w185{details?.PosterUrl}",
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

    public async Task ImportShows(string username, List<TraktShowResponse> shows)
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

                var showsProcessed = 0;
                foreach (var show in shows)
                {
                    var existingShow = context.Shows.FirstOrDefault(x => x.TMDB == show.Show.Ids.TMDB);
                    
                    var showDetails = await _detailsProvider.GetDetailsForShow(show.Show.Ids.TMDB);

                    if (existingShow == null)
                    {
                        existingShow = new ShowRecord
                        {
                            Identifier = Guid.NewGuid(),
                            Title = show.Show.Title,
                            Year = show.Show.Year,
                            TMDB = show.Show.Ids.TMDB,
                            Poster = $"https://image.tmdb.org/t/p/w185{showDetails?.PosterUrl}",
                            Overview = showDetails?.Overview,
                        };
                        
                        Console.WriteLine($"[INFO] - Show '{show.Show.Title}' doesn't exist. Creating...");
                        context.Add(existingShow);
                    }
                    
                    foreach (var season in show.Seasons)
                    {
                        var existingSeason = context.Seasons.FirstOrDefault(x =>
                            x.Number == season.Number &&
                            x.Show.Identifier == existingShow.Identifier
                        );

                        if (existingSeason == null)
                        {
                            var title = $"Season {season.Number}";

                            if (season.Number > 0 && showDetails?.Seasons.Count > (season.Number - 1))
                                title = showDetails.Seasons[season.Number - 1].Title;
                            
                            existingSeason = new SeasonRecord
                            {
                                Identifier = Guid.NewGuid(),
                                Number = season.Number,
                                Title = title,
                                Show = existingShow
                            };

                            Console.WriteLine($"[INFO] - Season '{season.Number}' for show '{show.Show.Title}' doesn't exist. Creating...");
                            context.Add(existingSeason);
                        }

                        foreach (var episode in season.Episodes)
                        {
                            var existingEpisode = context.Episodes.FirstOrDefault(x =>
                                x.Number == episode.Number &&
                                x.Season.Identifier == existingSeason.Identifier
                            );

                            if (existingEpisode == null)
                            {
                                var episodeDetails = await _detailsProvider.GetEpisodeDetails(show.Show.Ids.TMDB, season.Number, episode.Number); 
                                existingEpisode = new EpisodeRecord
                                {
                                    Identifier = Guid.NewGuid(),
                                    Number = episode.Number,
                                    Title = episodeDetails.Title ?? show.Show.Title,
                                    Season = existingSeason
                                };

                                Console.WriteLine($"[INFO] - Episode '{episodeDetails.Title ?? episode.Number.ToString()}' for season '{season.Number}' for show '{show.Show.Title}' doesn't exist. Creating...");
                                context.Add(existingEpisode);
                            }

                            var existingEpisodeUserRecord = context.EpisodeUserLinks.FirstOrDefault(x =>
                                x.User.Username.ToUpper() == username.ToUpper() &&
                                x.Episode.Number == episode.Number &&
                                x.Episode.Season.Identifier == existingSeason.Identifier &&
                                x.Episode.Season.Show.Identifier == existingShow.Identifier
                            );
                    
                            if ((existingEpisodeUserRecord?.WatchedAt - episode.WatchedAt)?.TotalHours > 1)
                                existingEpisodeUserRecord = null;

                            if (existingEpisodeUserRecord == null)
                            {
                                var episodeUserRecord = new EpisodeUserRecord
                                {
                                    Identifier = Guid.NewGuid(),
                                    User = existingUser,
                                    Episode = existingEpisode,
                                    WatchedAt = episode.WatchedAt
                                };
                                
                                Console.WriteLine($"[INFO] - Episode-User Link '{username}'-'{existingEpisode.Title}' doesn't exist. Creating...");
                                context.Add(episodeUserRecord);
                            }
                        }
                    }
                    
                    showsProcessed++;
                    Console.WriteLine($"[INFO] - Show {showsProcessed}/{shows.Count} processed.");
                }

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine($"[FATAL] - Failed to add a show. Exception below:");
                Console.WriteLine(exception.Message, exception.StackTrace);
                await transaction.RollbackAsync();
            }
        }
    }

    public List<Movie> GetAllMovies(string username)
    {
        
        using (var context = new DatabaseContext())
        {
            try
            {
                return context.MovieUserLinks
                    .Where(x => x.User.Username.ToUpper() == username.ToUpper())
                    .OrderByDescending(x => x.WatchedAt)
                    .Select(x => new Movie
                    {
                        Identifier = x.Identifier,
                        Title = x.Movie.Title,
                        Year = x.Movie.Year,
                        TMDB =  x.Movie.TMDB,
                        Poster = x.Movie.Poster,
                        Overview = x.Movie.Overview,
                        WatchedAt = x.WatchedAt
                    })
                    .ToList();
            }
            catch (Exception exception)
            {
                return new List<Movie>();
            }
        }
    }

    public List<Show> GetAllShows(string username)
    {
        
        using (var context = new DatabaseContext())
        {
            try
            {
                var shows = context.EpisodeUserLinks
                    .Where(x => x.User.Username.ToUpper() == username.ToUpper())
                    .Select(x => new Show
                    {
                        Identifier = x.Identifier,
                        Title = x.Episode.Title,
                        ParentTitle = x.Episode.Season.Title,
                        GrandParentTitle = x.Episode.Season.Show.Title,
                        Year = x.Episode.Season.Show.Year,
                        TMDB =  x.Episode.Season.Show.TMDB,
                        Poster = x.Episode.Season.Show.Poster,
                        Overview = x.Episode.Season.Show.Overview,
                        WatchedAt = x.WatchedAt,
                        SeasonNumber = x.Episode.Season.Number,
                        EpisodeNumber = x.Episode.Number,
                    })
                    .ToList();

                return shows
                    .OrderByDescending(x => x.WatchedAt)
                    .ToList();
            }
            catch (Exception exception)
            {
                return new List<Show>();
            }
        }
    }
    
    public Movie GetMovieByIdentifier(Guid identifier)
    {
        
        using (var context = new DatabaseContext())
        {
            try
            {
                var movie = context.MovieUserLinks
                    .Include(movieUserRecord => movieUserRecord.Movie)
                    .FirstOrDefault(x => x.Identifier.ToString().ToUpper() == identifier.ToString().ToUpper());

                if (movie == null)
                    return null;

                return new Movie
                {
                    Identifier = movie.Identifier,
                    Title = movie.Movie.Title,
                    Year = movie.Movie.Year,
                    TMDB =  movie.Movie.TMDB,
                    Poster = movie.Movie.Poster,
                    Overview = movie.Movie.Overview,
                    WatchedAt = movie.WatchedAt
                };
            }
            catch (Exception exception)
            {
                return null;
            }
        }
    }

    public Show GetShowByIdentifier(Guid identifier)
    {
        
        using (var context = new DatabaseContext())
        {
            try
            {
                var show = context.EpisodeUserLinks
                    .Include(episodeUserRecord => episodeUserRecord.Episode)
                    .ThenInclude(episodeRecord => episodeRecord.Season)
                    .ThenInclude(seasonRecord => seasonRecord.Show)
                    .FirstOrDefault(x => x.Identifier.ToString().ToUpper() == identifier.ToString().ToUpper());

                if (show == null)
                    return null;

                return new Show
                {
                    Identifier = show.Identifier,
                    Title = show.Episode.Title,
                    ParentTitle = show.Episode.Season.Title,
                    GrandParentTitle = show.Episode.Season.Show.Title,
                    Year = show.Episode.Season.Show.Year,
                    TMDB =  show.Episode.Season.Show.TMDB,
                    Poster = show.Episode.Season.Show.Poster,
                    Overview = show.Episode.Season.Show.Overview,
                    WatchedAt = show.WatchedAt,
                    SeasonNumber = show.Episode.Season.Number,
                    EpisodeNumber = show.Episode.Number,
                };
            }
            catch (Exception exception)
            {
                return null;
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

    public void ImportEpisode(string username, ShowRecord show, SeasonRecord season, EpisodeRecord episode)
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

                var existingShow = context.Shows.FirstOrDefault(x => x.TMDB == show.TMDB);

                if (existingShow == null)
                {
                    existingShow = show;
                    context.Add(existingShow);
                }

                var existingSeason = context.Seasons.FirstOrDefault(x => x.Show.TMDB == show.TMDB && x.Number == season.Number);

                if (existingSeason == null)
                {
                    existingSeason = season;
                    context.Add(existingSeason);
                }

                var existingEpisode = context.Episodes.FirstOrDefault(x => x.Season.Show.TMDB == show.TMDB && x.Number == episode.Number);

                if (existingEpisode == null)
                {
                    existingEpisode = episode;
                    context.Add(existingEpisode);
                }

                var existingEpisodeUserRecord = context.EpisodeUserLinks.FirstOrDefault(x =>
                    x.User.Username.ToUpper() == username.ToUpper() &&
                    x.Episode.Number == episode.Number &&
                    x.Episode.Season.Identifier == existingSeason.Identifier &&
                    x.Episode.Season.Show.Identifier == existingShow.Identifier
                );

                if (existingEpisodeUserRecord == null)
                {
                    var movieUserRecord = new EpisodeUserRecord
                    {
                        Identifier = Guid.NewGuid(),
                        User = existingUser,
                        Episode = episode,
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