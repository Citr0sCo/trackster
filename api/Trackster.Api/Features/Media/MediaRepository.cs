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
    void ImportMovie(string username, Movie movie);
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

                        context.Add(existingMovie);
                    }

                    var existingMovieUserRecord = context.MovieUserLinks.FirstOrDefault(x =>
                        x.User.Username.ToUpper() == username.ToUpper() &&
                        x.Movie.TMDB == movie.Movie.Ids.TMDB &&
                        x.WatchedAt == movie.LastWatchedAt
                    );

                    if (existingMovieUserRecord == null)
                    {
                        var movieUserRecord = new MovieUserRecord
                        {
                            Identifier = Guid.NewGuid(),
                            User = existingUser,
                            Movie = existingMovie,
                            WatchedAt = movie.LastWatchedAt
                        };

                        context.Add(movieUserRecord);
                    }
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

    public async Task ImportShows(string username, List<TraktShowResponse> shows)
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

                foreach (var show in shows)
                {
                    var existingShow = context.Shows.FirstOrDefault(x => x.TMDB == show.Show.Ids.TMDB);

                    if (existingShow == null)
                    {
                        var details = await _detailsProvider.GetDetailsForShow(show.Show.Ids.TMDB);
                        
                        existingShow = new ShowRecord
                        {
                            Identifier = Guid.NewGuid(),
                            Title = show.Show.Title,
                            Year = show.Show.Year,
                            TMDB = show.Show.Ids.TMDB,
                            Poster = $"https://image.tmdb.org/t/p/w185{details?.PosterUrl}",
                            Overview = details?.Overview,
                        };

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
                            existingSeason = new SeasonRecord
                            {
                                Identifier = Guid.NewGuid(),
                                Number = season.Number,
                                Show = existingShow
                            };

                            context.Add(existingSeason);
                        }

                        foreach (var episode in season.Episodes)
                        {
                            var existingEpisode = context.Episodes.FirstOrDefault(x =>
                                x.Number == season.Number &&
                                x.Season.Identifier == existingSeason.Identifier
                            );

                            if (existingEpisode == null)
                            {
                                existingEpisode = new EpisodeRecord
                                {
                                    Identifier = Guid.NewGuid(),
                                    Number = episode.Number,
                                    Season = existingSeason
                                };

                                context.Add(existingEpisode);
                            }

                            var existingEpisodeUserRecord = context.EpisodeUserLinks.FirstOrDefault(x =>
                                x.User.Username.ToUpper() == username.ToUpper() &&
                                x.Episode.Number == episode.Number &&
                                x.Episode.Season.Identifier == existingSeason.Identifier &&
                                x.Episode.Season.Show.Identifier == existingShow.Identifier &&
                                x.WatchedAt == episode.WatchedAt
                            );

                            if (existingEpisodeUserRecord == null)
                            {
                                var episodeUserRecord = new EpisodeUserRecord
                                {
                                    Identifier = Guid.NewGuid(),
                                    User = existingUser,
                                    Episode = existingEpisode,
                                    WatchedAt = episode.WatchedAt
                                };

                                context.Add(episodeUserRecord);
                            }
                        }
                    }
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
                        Title = x.Episode.Season.Show.Title,
                        Year = x.Episode.Season.Show.Year,
                        TMDB =  x.Episode.Season.Show.TMDB,
                        Poster = x.Episode.Season.Show.Poster,
                        Overview = x.Episode.Season.Show.Overview,
                        WatchedAt = x.WatchedAt
                    })
                    .GroupBy(x => x.Title)
                    .Select(x => x.First())
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

    public void ImportMovie(string username, Movie movie)
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
                    existingMovie = new MovieRecord
                    {
                        Identifier = Guid.NewGuid(),
                        Title = movie.Title,
                        Year = movie.Year,
                        TMDB = movie.TMDB,
                        Poster = $"https://image.tmdb.org/t/p/w185{movie.Poster}",
                        Overview = movie.Overview,
                    };

                    context.Add(existingMovie);
                }

                var existingMovieUserRecord = context.MovieUserLinks.FirstOrDefault(x =>
                    x.User.Username.ToUpper() == username.ToUpper() &&
                    x.Movie.TMDB == movie.TMDB &&
                    x.WatchedAt == movie.WatchedAt
                );

                if (existingMovieUserRecord == null)
                {
                    var movieUserRecord = new MovieUserRecord
                    {
                        Identifier = Guid.NewGuid(),
                        User = existingUser,
                        Movie = existingMovie,
                        WatchedAt = movie.WatchedAt
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