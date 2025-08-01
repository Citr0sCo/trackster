using Trackster.Api.Core.Helpers;
using Trackster.Api.Data;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Media.Importers.TraktImporter.Types;
using Trackster.Api.Features.Shows.Types;

namespace Trackster.Api.Features.Shows;

public interface IMediaRepository
{
    Task ImportShows(string username, List<TraktShowResponse> shows);
    List<WatchedShow> GetAllWatchedShows(string username);
    void ImportEpisode(string username, ShowRecord show, SeasonRecord season, EpisodeRecord episode);
    Show? GetShowBySlug(string slug);
    Season? GetSeasonByNumber(string slug, int seasonNumber);
    Episode? GetEpisodeByNumber(string slug, int seasonNumber, int episodeNumber);
    List<WatchedEpisode> GetWatchedHistoryByEpisodeNumber(string username, string slug, int seasonNumber, int episodeNumber);
}

public class ShowsRepository : IMediaRepository
{
    private readonly TmdbImportProvider _detailsProvider;

    public ShowsRepository()
    {
        _detailsProvider = new TmdbImportProvider();
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
                            Slug = SlugHelper.GenerateSlugFor(show.Show.Title),
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

    public List<WatchedShow> GetAllWatchedShows(string username)
    {
        
        using (var context = new DatabaseContext())
        using (var transaction = context.Database.BeginTransaction())
        {
            try
            {
                var watchedShows = context.EpisodeUserLinks
                    .Where(x => x.User.Username.ToUpper() == username.ToUpper())
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
                
                foreach (var watchedShow in watchedShows)
                {
                    if (!string.IsNullOrEmpty(watchedShow.Show.Slug))
                        continue;
                    
                    watchedShow.Show.Slug = SlugHelper.GenerateSlugFor(watchedShow.Show.Title);
                    
                    var show = context.Shows.FirstOrDefault(x => x.Identifier == watchedShow.Show.Identifier);
                    show.Slug = watchedShow.Show.Slug;
                    context.Update(show);
                }
            
                context.SaveChanges();
                transaction.Commit();

                return watchedShows
                    .OrderByDescending(x => x.WatchedAt)
                    .ToList();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                transaction.Rollback();
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
            catch (Exception exception)
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
            catch (Exception exception)
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
            catch (Exception exception)
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
            catch (Exception exception)
            {
                return new  List<WatchedEpisode>();
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