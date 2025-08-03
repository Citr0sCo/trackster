using Trackster.Api.Data;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Shows.Types;

namespace Trackster.Api.Features.Shows;

public interface IShowsRepository
{
    Task<ShowRecord?> GetShowByTmdbId(string tmdbId);
    Task<SeasonRecord?> GetSeasonBy(int seasonNumber, Guid showIdentifier);
    Task<EpisodeRecord?> GetEpisodeBy(int episodeNumber, Guid seasonIdentifier);
    List<WatchedShow> GetAllWatchedShows(string username, int results, int page);
    Show? GetShowBySlug(string slug);
    Season? GetSeasonByNumber(string slug, int seasonNumber);
    Episode? GetEpisodeByNumber(string slug, int seasonNumber, int episodeNumber);
    List<WatchedEpisode> GetWatchedHistoryByEpisodeNumber(string username, string slug, int seasonNumber, int episodeNumber);
    Task ImportShow(UserRecord user, ShowRecord show);
    Task ImportSeason(UserRecord user, ShowRecord show, SeasonRecord season);
    Task ImportEpisode(UserRecord user, ShowRecord show, SeasonRecord season, EpisodeRecord episode);
    Task MarkEpisodeAsWatched(string username, string showTmdbId, int seasonNumber, int episodeNumber, DateTime watchedAt);
}

public class ShowsRepository : IShowsRepository
{
    public async Task<ShowRecord?> GetShowByTmdbId(string tmdbId)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                return context.Shows.FirstOrDefault(x => x.TMDB == tmdbId);
            }
            catch (Exception)
            {
                Console.WriteLine($"[FATAL] - Failed to get show.");
                return null;
            }
        }
    }

    public async Task<SeasonRecord?> GetSeasonBy(int seasonNumber, Guid showIdentifier)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                return context.Seasons.FirstOrDefault(x =>
                    x.Number == seasonNumber &&
                    x.Show.Identifier == showIdentifier
                );
            }
            catch (Exception)
            {
                Console.WriteLine($"[FATAL] - Failed to get season.");
                return null;
            }
        }
    }

    public async Task<EpisodeRecord?> GetEpisodeBy(int episodeNumber, Guid seasonIdentifier)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                return  context.Episodes.FirstOrDefault(x =>
                    x.Number == episodeNumber &&
                    x.Season.Identifier == seasonIdentifier
                );
            }
            catch (Exception)
            {
                Console.WriteLine($"[FATAL] - Failed to get episode.");
                return null;
            }
        }
    }

    public List<WatchedShow> GetAllWatchedShows(string username, int results, int page)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                return context.EpisodeUserLinks
                    .Where(x => x.User.Username.ToUpper() == username.ToUpper())
                    .OrderByDescending(x => x.WatchedAt)
                    .Skip((page - 1) * results)
                    .Take(results)
                    .Select(x => new WatchedShow
                    {
                        Show = new Show
                        {
                            Identifier = x.Episode.Season.Show.Identifier,
                            Title = x.Episode.Season.Show.Title,
                            Slug = x.Episode.Season.Show.Slug,
                            Year = x.Episode.Season.Show.Year,
                            TMDB =  x.Episode.Season.Show.TMDB,
                            Poster = x.Episode.Season.Show.Poster,
                            Overview = x.Episode.Season.Show.Overview,
                        },
                        Season = new Season
                        {
                            Identifier = x.Episode.Season.Identifier,
                            Number = x.Episode.Season.Number,
                            Title = x.Episode.Season.Title,
                        },
                        Episode = new Episode
                        {
                            Identifier = x.Episode.Identifier,
                            Number = x.Episode.Number,
                            Title = x.Episode.Title,
                        },
                        WatchedAt = x.WatchedAt
                    })
                    .ToList();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        return new List<WatchedShow>();
    }

    public Show? GetShowBySlug(string slug)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                var show = context.Shows
                    .FirstOrDefault(x => x.Slug.ToUpper() == slug.ToUpper());

                if (show == null)
                    return null;

                return new Show
                {
                    Identifier = show.Identifier,
                    Title = show.Title,
                    Slug = show.Slug,
                    Year = show.Year,
                    TMDB =  show.TMDB,
                    Poster = show.Poster,
                    Overview = show.Overview
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public Season? GetSeasonByNumber(string slug, int seasonNumber)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                var season = context.Seasons
                    .Where(x => x.Show.Slug.ToUpper() == slug.ToUpper())
                    .FirstOrDefault(x => x.Number == seasonNumber);

                if (season == null)
                    return null;

                return new Season
                {
                    Identifier = season.Identifier,
                    Title = season.Title,
                    Number = season.Number
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public Episode? GetEpisodeByNumber(string slug, int seasonNumber, int episodeNumber)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                var episode = context.Episodes
                    .Where(x => x.Season.Show.Slug.ToUpper() == slug.ToUpper())
                    .Where(x => x.Season.Number == seasonNumber)
                    .FirstOrDefault(x => x.Number == episodeNumber);

                if (episode == null)
                    return null;

                return new Episode
                {
                    Identifier = episode.Identifier,
                    Title = episode.Title,
                    Number = episode.Number
                };
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

    public List<WatchedEpisode> GetWatchedHistoryByEpisodeNumber(string username, string slug, int seasonNumber, int episodeNumber)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                var watchHistory = context.EpisodeUserLinks
                    .Where(x => x.User.Username.ToUpper() == username.ToUpper())
                    .Where(x => x.Episode.Season.Show.Slug.ToUpper() == slug.ToUpper())
                    .Where(x => x.Episode.Season.Number == seasonNumber)
                    .Where(x => x.Episode.Number == episodeNumber)
                    .ToList();

                if (watchHistory == null)
                    return null;

                return watchHistory.ConvertAll((episode) =>
                {
                    return new WatchedEpisode
                    {
                        WatchedAt = episode.WatchedAt
                    };
                });
            }
            catch (Exception)
            {
                return new  List<WatchedEpisode>();
            }
        }
    }

    public async Task ImportShow(UserRecord user, ShowRecord show)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var existingUser = context.Users.FirstOrDefault(x => x.Username.ToUpper() == user.Username.ToUpper());
                if (existingUser == null)
                    context.Add(user);

                var existingShow = context.Shows.FirstOrDefault(x => x.TMDB == show.TMDB);
                if (existingShow == null)
                    context.Add(show);

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

    public async Task ImportSeason(UserRecord user, ShowRecord show, SeasonRecord season)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var existingUser = context.Users.FirstOrDefault(x => x.Username.ToUpper() == user.Username.ToUpper());
                if (existingUser == null)
                    context.Add(user);

                var existingShow = context.Shows.FirstOrDefault(x => x.TMDB == show.TMDB);
                if (existingShow == null)
                    context.Add(show);

                var existingSeason = context.Seasons.FirstOrDefault(x => x.Show.TMDB == show.TMDB && x.Number == season.Number);
                if (existingSeason == null)
                {
                    season.Show = existingShow ?? show;
                    context.Add(season);
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

    public async Task ImportEpisode(UserRecord user, ShowRecord show, SeasonRecord season, EpisodeRecord episode)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var existingUser = context.Users.FirstOrDefault(x => x.Username.ToUpper() == user.Username.ToUpper());
                if (existingUser == null)
                    context.Add(user);

                var existingShow = context.Shows.FirstOrDefault(x => x.TMDB == show.TMDB);
                if (existingShow == null)
                    context.Add(show);

                var existingSeason = context.Seasons.FirstOrDefault(x => x.Show.TMDB == show.TMDB 
                                                                         && x.Number == season.Number);
                if (existingSeason == null)
                {
                    season.Show = existingShow ?? show;
                    context.Add(season);
                }

                var existingEpisode = context.Episodes.FirstOrDefault(x => x.Season.Show.TMDB == show.TMDB 
                                                                           && x.Season.Number == season.Number
                                                                           && x.Number == episode.Number);
                if (existingEpisode == null)
                {
                    episode.Season = existingSeason ?? season;
                    episode.Season.Show = existingShow ?? show;
                    context.Add(episode);
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

    public async Task MarkEpisodeAsWatched(string username, string showTmdbId, int seasonNumber, int episodeNumber, DateTime watchedAt)
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

                var existingShow = context.Shows.FirstOrDefault(x => x.TMDB == showTmdbId);

                if (existingShow == null)
                {
                    Console.WriteLine($"[INFO] - Show '{showTmdbId}' doesn't exist.");
                    return;
                }

                var existingSeason = context.Seasons.FirstOrDefault(x => x.Show.TMDB == showTmdbId 
                                                                         && x.Number == seasonNumber);

                if (existingSeason == null)
                {
                    Console.WriteLine($"[INFO] - Season '{seasonNumber}' doesn't exist.");
                    return;
                }

                var existingEpisode = context.Episodes.FirstOrDefault(x => x.Season.Show.TMDB == showTmdbId 
                                                                         && x.Season.Number == seasonNumber 
                                                                         && x.Number == episodeNumber);

                if (existingEpisode == null)
                {
                    Console.WriteLine($"[INFO] - Episode '{episodeNumber}' doesn't exist.");
                    return;
                }
                
                var existingEpisodeUserRecord = context.EpisodeUserLinks.FirstOrDefault(x =>
                    x.User.Username.ToUpper() == username.ToUpper() &&
                    x.Episode.Season.Show.TMDB == showTmdbId && 
                    x.Episode.Season.Number == seasonNumber && 
                    x.Episode.Number == episodeNumber &&
                    x.WatchedAt >= watchedAt.AddHours(-1) &&
                    x.WatchedAt <= watchedAt.AddHours(1)
                );

                if (existingEpisodeUserRecord == null)
                {
                    var movieUserRecord = new EpisodeUserRecord
                    {
                        Identifier = Guid.NewGuid(),
                        User = existingUser,
                        Episode = existingEpisode,
                        WatchedAt = watchedAt,
                    };

                    Console.WriteLine($"[INFO] - Episode-User Link '{username}'-'{existingEpisode.Title}' doesn't exist. Creating...");
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
}