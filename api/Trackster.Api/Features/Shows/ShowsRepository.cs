using Microsoft.EntityFrameworkCore;
using Trackster.Api.Data;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Shows.Types;

namespace Trackster.Api.Features.Shows;

public interface IShowsRepository
{
    Task<ShowRecord?> GetShowByTmdbId(string tmdbId);
    Task<SeasonRecord?> GetSeasonBy(int seasonNumber, Guid showIdentifier);
    Task<EpisodeRecord?> GetEpisodeBy(int episodeNumber, Guid seasonIdentifier);
    List<WatchedEpisode> GetAllWatchedEpisodes(string username, int results, int page);
    Show? GetShowBySlug(string slug);
    Season? GetSeasonByNumber(string slug, int seasonNumber);
    Episode? GetEpisodeByNumber(string slug, int seasonNumber, int episodeNumber);
    List<WatchedEpisode> GetWatchedHistoryByEpisodeNumber(string username, string slug, int seasonNumber, int episodeNumber);
    Task ImportShow(UserRecord user, ShowRecord show, List<GenreRecord> genres);
    Task ImportSeason(UserRecord user, ShowRecord show, SeasonRecord season);
    Task ImportEpisode(UserRecord user, ShowRecord show, SeasonRecord season, EpisodeRecord episode);
    Task MarkEpisodeAsWatched(UserRecord user, ShowRecord show, SeasonRecord season, EpisodeRecord episode, DateTime watchedAt);
    EpisodeUserRecord? GetWatchedShowByLastWatchedAt(string username, string tmdbId, DateTime watchedAt);
    ShowRecord? GetShowByReference(Guid identifier);
    SeasonRecord? GetSeasonByReference(Guid identifier);
    Task<EpisodeRecord> UpdateEpisode(EpisodeRecord episodeRecord, List<GenreRecord> genres);
    Task SaveEpisode(ShowRecord show, SeasonRecord season, EpisodeRecord episode);
    Task<List<GenreRecord>> FindOrCreateGenres(List<string> genres);
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

    public List<WatchedEpisode> GetAllWatchedEpisodes(string username, int results, int page)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                var userEpisodes = context.EpisodeUserLinks
                    .Include(x => x.Episode)
                    .ThenInclude(x => x.Season)
                    .ThenInclude(x => x.Show)
                    .Where(x => x.User.Username.ToUpper() == username.ToUpper())
                    .OrderByDescending(x => x.WatchedAt)
                    .Skip((page - 1) * results)
                    .Take(results)
                    .ToList();
                
                var showIds = userEpisodes
                    .Select(x => x.Episode.Season.Show.Identifier)
                    .Distinct()
                    .ToList();
                
                var genres = context.ShowGenres
                    .Include(showGenreRecord => showGenreRecord.Show)
                    .Include(showGenreRecord => showGenreRecord.Genre)
                    .Where(x => showIds.Contains(x.Show.Identifier))
                    .ToList();
                
                return userEpisodes
                    .Select(x => new WatchedEpisode
                    {
                        Episode = ShowMapper.MapEpisode(x.Episode, genres.Where(y => y.Show.Identifier == x.Episode.Season.Show.Identifier).Select(x => x.Genre).ToList()),
                        WatchedAt = x.WatchedAt
                    })
                    .ToList();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        return new List<WatchedEpisode>();
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

                var genres = context.ShowGenres
                    .Include(x => x.Genre)
                    .Include(x => x.Show)
                    .Where(x => x.Show.Identifier == show.Identifier)
                    .Select(x => x.Genre)
                    .ToList();

                return ShowMapper.Map(show, genres);
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
                    .Include(x => x.Show)
                    .Where(x => x.Show.Slug.ToUpper() == slug.ToUpper())
                    .FirstOrDefault(x => x.Number == seasonNumber);

                if (season == null)
                    return null;

                var genres = context.ShowGenres
                    .Include(x => x.Genre)
                    .Include(x => x.Show)
                    .Where(x => x.Show.Identifier == season.Show.Identifier)
                    .Select(x => x.Genre)
                    .ToList();

                return ShowMapper.MapSeason(season, genres);
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
                    .Include(episodeRecord => episodeRecord.Season)
                    .ThenInclude(seasonRecord => seasonRecord.Show)
                    .FirstOrDefault(x => x.Number == episodeNumber);

                if (episode == null)
                    return null;

                var genres = context.ShowGenres
                    .Include(x => x.Genre)
                    .Include(x => x.Show)
                    .Where(x => x.Show.Identifier == episode.Season.Show.Identifier)
                    .Select(x => x.Genre)
                    .ToList();

                return ShowMapper.MapEpisode(episode, genres);
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

    public async Task ImportShow(UserRecord user, ShowRecord show, List<GenreRecord> genres)
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

                var existingShow = context.Shows.FirstOrDefault(x => x.TMDB == show.TMDB);
                
                if (existingShow == null)
                {
                    Console.WriteLine($"[INFO] - Show '{show.Title}' doesn't exist. Creating...");
                    context.Add(show);
                }
                
                foreach (var genre in genres)
                {
                    var existingGenre = context.Genres.FirstOrDefault(x => x.Identifier == genre.Identifier);

                    if (existingGenre == null)
                    {    
                        Console.WriteLine($"[INFO] - Genre '{genre.Name}' doesn't exist. Creating...");
                        context.Add(genre);
                    }
                    
                    var existingShowGenre = context.ShowGenres.FirstOrDefault(x => x.Show.Identifier == show.Identifier && x.Genre.Identifier == genre.Identifier);

                    if (existingShowGenre != null)
                        continue;
                    
                    context.ShowGenres.Add(new ShowGenreRecord
                    {
                        Identifier = Guid.NewGuid(),
                        Show = existingShow ?? show,
                        Genre = existingGenre ?? genre,
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

    public async Task SaveEpisode(ShowRecord show, SeasonRecord season, EpisodeRecord episode)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
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

    public async Task MarkEpisodeAsWatched(UserRecord user, ShowRecord show, SeasonRecord season, EpisodeRecord episode, DateTime watchedAt)
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

                var existingShow = context.Shows.FirstOrDefault(x => x.TMDB == show.TMDB);

                if (existingShow == null)
                {
                    Console.WriteLine($"[INFO] - Show '{show.TMDB}' doesn't exist. Creating...");

                    existingShow = show;
                    context.Shows.Add(existingShow);
                }

                var existingSeason = context.Seasons.FirstOrDefault(x => x.Show.TMDB == show.TMDB 
                                                                         && x.Number == season.Number);

                if (existingSeason == null)
                {
                    Console.WriteLine($"[INFO] - Season '{season.Number}' doesn't exist. Creating...");
                    
                    existingSeason = season;
                    context.Seasons.Add(existingSeason);
                }

                var existingEpisode = context.Episodes.FirstOrDefault(x => x.Season.Show.TMDB == show.TMDB 
                                                                         && x.Season.Number == season.Number
                                                                         && x.Number == episode.Number);

                if (existingEpisode == null)
                {
                    Console.WriteLine($"[INFO] - Episode '{episode.Number}' doesn't exist. Creating...");
                    
                    existingEpisode = episode;
                    context.Episodes.Add(existingEpisode);
                }
                
                var existingEpisodeUserRecord = context.EpisodeUserLinks.FirstOrDefault(x =>
                    x.User.Username.ToUpper() == user.Username.ToUpper() &&
                    x.Episode.Season.Show.TMDB == show.TMDB && 
                    x.Episode.Season.Number == season.Number && 
                    x.Episode.Number == episode.Number &&
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

                    Console.WriteLine($"[INFO] - Episode-User Link '{user.Username}'-'{existingEpisode.Title}' doesn't exist. Creating...");
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

    public EpisodeUserRecord? GetWatchedShowByLastWatchedAt(string username, string tmdbId, DateTime watchedAt)
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

                var existingShow = context.Shows.FirstOrDefault(x => x.TMDB == tmdbId);

                if (existingShow == null)
                {
                    Console.WriteLine($"[INFO] - Show '{tmdbId}' doesn't exist.");
                    return null;
                }
                
                var existingEpisodeUserRecord = context.EpisodeUserLinks.FirstOrDefault(x =>
                    x.User.Username.ToUpper() == username.ToUpper() &&
                    x.Episode.Season.Show.TMDB.ToUpper() == tmdbId.ToUpper() &&
                    x.WatchedAt >= watchedAt.AddHours(-1) &&
                    x.WatchedAt <= watchedAt.AddHours(1)
                );

                return existingEpisodeUserRecord;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
            }
        }

        return null;
    }

    public ShowRecord? GetShowByReference(Guid identifier)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                return context.Shows.FirstOrDefault(x => x.Identifier.ToString().ToUpper() == identifier.ToString().ToUpper());
            }
            catch (Exception)
            {
                Console.WriteLine($"[FATAL] - Failed to get show.");
                return null;
            }
        }
    }

    public SeasonRecord? GetSeasonByReference(Guid identifier)
    {
        using (var context = new DatabaseContext())
        {
            try
            {
                return context.Seasons.FirstOrDefault(x => x.Identifier.ToString().ToUpper() == identifier.ToString().ToUpper());
            }
            catch (Exception)
            {
                Console.WriteLine($"[FATAL] - Failed to get show.");
                return null;
            }
        }
    }

    public async Task<EpisodeRecord> UpdateEpisode(EpisodeRecord episodeRecord, List<GenreRecord> genres)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var existingEpisode = context.Episodes
                    .Include(x => x.Season)
                    .ThenInclude(x => x.Show)
                    .FirstOrDefault(x => x.Identifier.ToString().ToUpper() == episodeRecord.Identifier.ToString().ToUpper());

                if (existingEpisode == null)
                    return episodeRecord;
                
                existingEpisode.Title = episodeRecord.Title;
                
                foreach (var genre in genres)
                {
                    var existingGenre = context.Genres.FirstOrDefault(x => x.Identifier == genre.Identifier);

                    if (existingGenre == null)
                        continue;
                    
                    var existingMovieGenre = context.ShowGenres.FirstOrDefault(x => x.Show.Identifier == existingEpisode.Season.Show.Identifier && x.Genre.Identifier == genre.Identifier);

                    if (existingMovieGenre != null)
                        continue;

                    var showGenreRecord = new ShowGenreRecord
                    {
                        Identifier = Guid.NewGuid(),
                        Show = existingEpisode.Season.Show,
                        Genre = existingGenre,
                    };
                    
                    context.ShowGenres.Add(showGenreRecord);
                }
                
                context.Episodes.Update(existingEpisode);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return existingEpisode;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                await transaction.RollbackAsync();
            }

            return episodeRecord;
        }
    }
}