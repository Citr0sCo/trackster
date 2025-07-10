using Trackster.Api.Data;
using Trackster.Api.Features.Media.Importers.TraktImporter.Types;

namespace Trackster.Api.Features.Media;

public interface IMediaRepository
{
    void ImportMovies(string username, List<TraktMovieResponse> movies);
    void ImportShows(string username, List<TraktShowResponse> shows);
}

public class MediaRepository : IMediaRepository
{
    public void ImportMovies(string username, List<TraktMovieResponse> movies)
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
                        existingMovie = new MovieRecord
                        {
                            Identifier = Guid.NewGuid(),
                            Title = movie.Movie.Title,
                            Year = movie.Movie.Year,
                            TMDB = movie.Movie.Ids.TMDB
                        };

                        context.Add(existingMovie);
                    }

                    var existingMovieUserRecord = context.MovieUserLinks.FirstOrDefault(x =>
                        x.User.Username.ToUpper() == username.ToUpper() &&
                        x.Movie.TMDB == movie.Movie.Ids.TMDB &&
                        x.CollectedAt == movie.CollectedAt
                    );

                    if (existingMovieUserRecord == null)
                    {
                        var movieUserRecord = new MovieUserRecord
                        {
                            Identifier = Guid.NewGuid(),
                            User = existingUser,
                            Movie = existingMovie,
                            CollectedAt = movie.CollectedAt
                        };

                        context.Add(movieUserRecord);
                    }
                }

                context.SaveChanges();
                transaction.Commit();
            }
            catch (Exception exception)
            {
                transaction.Rollback();
            }
        }
    }

    public void ImportShows(string username, List<TraktShowResponse> shows)
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
                        existingShow = new ShowRecord
                        {
                            Identifier = Guid.NewGuid(),
                            Title = show.Show.Title,
                            Year = show.Show.Year,
                            TMDB = show.Show.Ids.TMDB
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
                                x.CollectedAt == episode.CollectedAt
                            );

                            if (existingEpisodeUserRecord == null)
                            {
                                var episodeUserRecord = new EpisodeUserRecord
                                {
                                    Identifier = Guid.NewGuid(),
                                    User = existingUser,
                                    Episode = existingEpisode,
                                    CollectedAt = episode.CollectedAt
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
                transaction.Rollback();
            }
        }
    }
}