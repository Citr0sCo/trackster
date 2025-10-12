using System.Diagnostics;
using Newtonsoft.Json;
using Trackster.Api.Core.Helpers;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Auth.Providers.Trakt;
using Trackster.Api.Features.Media.Importers.OverseerrImporter;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Media.Importers.TmdbImporter.Types;
using Trackster.Api.Features.Media.Importers.TraktImporter;
using Trackster.Api.Features.Media.Importers.TraktImporter.Types;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.Movies;
using Trackster.Api.Features.Notifications;
using Trackster.Api.Features.Sessions;
using Trackster.Api.Features.Shows;
using Trackster.Api.Features.Users;
using Trackster.Api.Features.Webhooks.Types;

namespace Trackster.Api.Features.Media;

public class MediaService
{
    private readonly IMoviesService _moviesService;
    private readonly IShowsService _showsService;
    private readonly IUsersService _usersService;
    private readonly TraktImportProvider _traktProvider;
    private readonly WatchingNowService _watchingNowService;
    private readonly TmdbImportProvider _detailsProvider;
    private readonly NotificationsService _notificationsService;
    private readonly OverseerrService _overseerrService;

    private const string MOVIE_MEDIA_TYPE = "movie";
    private string EPISODE_MEDIA_TYPE = "episode";

    public MediaService(IMoviesService moviesService, IShowsService showsService, IUsersService usersService, ISessionService sessionService)
    {
        _moviesService = moviesService;
        _showsService = showsService;
        _usersService = usersService;
        _traktProvider = new TraktImportProvider(usersService, sessionService);
        _watchingNowService = WatchingNowService.Instance();
        _detailsProvider = new TmdbImportProvider();
        _notificationsService = new NotificationsService();
        _overseerrService = new OverseerrService();
    }

    public async IAsyncEnumerable<ImportMediaResponse> ImportMedia(ImportMediaRequest request)
    {
        if (request.Type == ImportType.Trakt && request.Username != null)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Console.WriteLine("[INFO] - Starting Trakt import...");
            yield return new ImportMediaResponse
            {
                Data = "[INFO] - Starting Trakt import..."
            };
            
            var movies = await _traktProvider.GetMovies(request.Username, request.Debug);
            var shows = await _traktProvider.GetShows(request.Username, request.Debug);

            var value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (value == "Development")
            {
                movies = movies.OrderByDescending(x => x.LastWatchedAt).Take(5).ToList();
                shows = shows.OrderByDescending(x => x.LastWatchedAt).Take(5).ToList();
            }

            var user = await ProcessUser(request);

            await foreach (var importMediaResponse in ProcessMovies(request, movies, user, request.Debug)) 
                yield return importMediaResponse;

            await foreach (var importMediaResponse in ProcessShows(request, shows, user, request.Debug))
                yield return importMediaResponse;            
            
            stopwatch.Stop();
            
            var time = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);
            
            Console.WriteLine($"[INFO] - Finished Trakt import in {time.Minutes}m {time.Seconds}s!");
            yield return new ImportMediaResponse
            {
                Data = $"[INFO] - Finished Trakt import in {time.Minutes}m {time.Seconds}s!"
            };
        }
        
        yield return new ImportMediaResponse
        {
            Data = "[INFO] - DONE!"
        };
    }

    public GetHistoryForUserResponse GetHistoryForUser(string username, int results, int page)
    {
        var movies = _moviesService.GetAllWatchedMovies(username, results, page);
        var shows = _showsService.GetAllWatchedEpisodes(username, results, page);

        var media = new List<Types.Media>();

        foreach (var movie in movies.WatchedMovies)
        {
            media.Add(new Types.Media
            {
                Identifier = movie.Movie.Identifier,
                Title = movie.Movie.Title,
                Slug = movie.Movie.Slug,
                Year = movie.Movie.Year,
                Overview = movie.Movie.Overview,
                Poster = movie.Movie.Poster,
                TMDB = movie.Movie.TMDB,
                Type = MediaType.Movie.ToString(),
                WatchedAt = movie.WatchedAt,
            });
        }

        foreach (var show in shows.WatchedEpisodes)
        {
            media.Add(new Types.Media
            {
                Identifier = show.Episode.Season.Show.Identifier,
                Title = show.Episode.Title,
                ParentTitle = show.Episode.Season.Title,
                GrandParentTitle = show.Episode.Season.Show.Title,
                Slug = show.Episode.Season.Show.Slug,
                Year = show.Episode.Season.Show.Year,
                Overview = show.Episode.Season.Show.Overview,
                Poster = show.Episode.Season.Show.Poster,
                TMDB = show.Episode.Season.Show.TMDB,
                Type = MediaType.Episode.ToString(),
                SeasonNumber = show.Episode.Season.Number,
                EpisodeNumber = show.Episode.Number,
                WatchedAt = show.WatchedAt,
            });
        }

        return new GetHistoryForUserResponse
        {
            Media = media.OrderByDescending(x => x.WatchedAt).ToList()
        };
    }

    public GetStatsForCalendarResonse GetStatsForCalendar(string username, int daysInThePast)
    {
        var movies = _moviesService.GetAllWatchedMovies(username, 10000, 1);
        var shows = _showsService.GetAllWatchedEpisodes(username, 10000, 1);

        var stats = new Dictionary<string, int>();

        var lowestDate = DateTime.Now.AddDays(-daysInThePast);

        foreach (var movie in movies.WatchedMovies)
        {
            if(movie.WatchedAt < lowestDate)
                continue;
            
            var key = GenerateKeyFromDate(movie.WatchedAt);
            
            if(!stats.TryAdd(key, 1))
                stats[key]++;
        }

        foreach (var show in shows.WatchedEpisodes)
        {
            if(show.WatchedAt < lowestDate)
                continue;
            
            var key = GenerateKeyFromDate(show.WatchedAt);
            
            if(!stats.TryAdd(key, 1))
                stats[key]++;
        }

        return new GetStatsForCalendarResonse
        {
            Stats = stats
        };
    }

    private static string GenerateKeyFromDate(DateTime date)
    {
        var month = date.Month.ToString();
        if (date.Month < 10)
            month = $"0{date.Month}";

        var day = date.Day.ToString();
        if (date.Day < 10)
            day = $"0{date.Day}";
        
        return $"{date.Year}-{month}-{day}";
    }

    public GetStatsResponse GetStats(string username)
    {
        var movies = _moviesService.GetAllWatchedMovies(username, 10000, 1);
        var shows = _showsService.GetAllWatchedEpisodes(username, 10000, 1);

        return new GetStatsResponse
        {
            Total = movies.WatchedMovies.Count + shows.WatchedEpisodes.Count,
            MoviesWatched = movies.WatchedMovies.Count,
            EpisodesWatched = shows.WatchedEpisodes.Count
        };
    }

    public async Task MarkMediaAsWatched(MarkMediaAsWatchedRequest request)
    {
        if (request.MediaType == MOVIE_MEDIA_TYPE)
            await MarkMovieAsWatched(request.Username, request.Title, request.Year, request.RequestDebug);

        if (request.MediaType == EPISODE_MEDIA_TYPE)
            await MarkEpisodeAsWatched(request.Username, request.GrandParentTitle, request.ParentTitle, request.Title, request.Year, request.SeasonNumber);
    }

    public async Task MarkMediaAsWatchingNow(MarkMediaAsWatchingNowRequest request)
    {
        if (request.MediaType == MOVIE_MEDIA_TYPE)
        {
            var movie = await _moviesService.SearchForMovieBy(request.Title, request.Year, request.Debug);
            Console.WriteLine($"[INFO] - Making movie as watching now. Title: {request.Title}, Year: {request.Year}, Looked Up MovieTmdbId: {movie.TMDB}.");
            
            if (movie.TMDB?.Length == 0)
                return;
            
            _watchingNowService.MarkAsWatchingMovie(request.UserIdentifier, movie, request.MillisecondsWatched, request.Duration);
        }

        if (request.MediaType == EPISODE_MEDIA_TYPE)
        {
            var episode = await _showsService.SearchForEpisode(request.GrandParentTitle, request.ParentTitle, request.Title, request.Year, request.SeasonNumber, request.Debug);
            Console.WriteLine($"[INFO] - Making episode as watching now. Title: {request.Title}, Year: {request.Year}, Looked Up Episode Title: {episode.Title}.");

            if (episode.Title?.Length == 0)
                return;
            
            _watchingNowService.MarkAsWatchingEpisode(request.UserIdentifier, episode, request.MillisecondsWatched, request.Duration);
        }

        Console.WriteLine($"Marking a media as watching now. {request.Title}, {request.GrandParentTitle}, {request.SeasonNumber}, {request.Year}.");
    }

    public void RemoveMediaAsWatchingNow(Guid userReference, string mediaType)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
            _watchingNowService.MarkAsStoppedWatchingMovie(userReference);

        if (mediaType == EPISODE_MEDIA_TYPE)
            _watchingNowService.MarkAsStoppedWatchingEpisode(userReference);

        Console.WriteLine($"Marking a media as stopped watching.");
    }

    public async Task PauseMediaAsWatchingNow(MarkMediaAsPausedRequest request)
    {
        if (request.MediaType == MOVIE_MEDIA_TYPE)
        {
            var movie = await _moviesService.SearchForMovieBy(request.Title, request.Year, request.Debug);
            _watchingNowService.MarkAsPausedWatchingMovie(request.UserIdentifier, movie, request.MillisecondsWatched, request.Duration);
        }

        if (request.MediaType == EPISODE_MEDIA_TYPE)
        {
            var episode = await _showsService.SearchForEpisode(request.GrandParentTitle, request.ParentTitle, request.Title, request.Year, request.SeasonNumber, request.Debug);
            _watchingNowService.MarkAsPausedWatchingEpisode(request.UserIdentifier, episode, request.MillisecondsWatched, request.Duration);
        }

        Console.WriteLine($"Marking a media as paused watching. {request.Title}, {request.GrandParentTitle}, {request.SeasonNumber}, {request.Year}.");
    }

    private async Task MarkMovieAsWatched(string username, string title, int year, bool requestDebug)
    {
        try
        {
            var user = await _usersService.GetUserByUsername(username);
            var movie = await _moviesService.SearchForMovieBy(title, year, requestDebug);
            await _moviesService.MarkMovieAsWatched(user, movie, DateTime.UtcNow);
            await _notificationsService.Send($"Movie '{title} ({year})' marked as watched.");
        }
        catch (Exception ex)
        {
            await _notificationsService.Send($"⚠️ Failed to mark Movie '{title} ({year})' as watched.");
            Console.WriteLine($"[FATAL] - Failed to mark movie as watched. Exception: {ex.Message}.");
            Console.WriteLine(ex);
        }
    }

    private async Task MarkEpisodeAsWatched(string username, string showTitle, string seasonTitle, string episodeTitle, int year, int seasonNumber, bool requestDebug = false)
    {
        try
        {
            var user = await _usersService.GetUserByUsername(username);
            var episode = await _showsService.SearchForEpisode(showTitle, seasonTitle, episodeTitle, year, seasonNumber, requestDebug);
            await _showsService.MarkEpisodeAsWatched(user, episode.Season.Show, episode.Season, episode, DateTime.Now);
            await _notificationsService.Send($"Episode '{episodeTitle}' of show '{showTitle}' marked as watched.");
        }
        catch (Exception ex)
        {
            await _notificationsService.Send($"⚠️ Failed to mark Episode '{episodeTitle}' of show '{showTitle}' as watched.");
            Console.WriteLine($"[FATAL] - Failed to mark episode as watched. Exception: {ex.Message}.");
            Console.WriteLine(ex);
        }
    }

    private async Task<UserRecord> ProcessUser(ImportMediaRequest request)
    {
        var userRecord = await _usersService.GetUserByUsername(request.Username);

        if (userRecord == null)
        {
            throw new Exception($"User '{request.Username}' does not exist.");
        }

        return userRecord;
    }
    
    private async IAsyncEnumerable<ImportMediaResponse> ProcessMovies(ImportMediaRequest request, List<TraktMovieResponse> movies, UserRecord userRecord, bool requestDebug)
    {
        Console.WriteLine($"[INFO] - Will process {movies.Count} movies.");
        yield return new ImportMediaResponse
        {
            Data = $"[INFO] - Will process {movies.Count} movies."
        };

        var processedMovies = 0;
        foreach (var movie in movies)
        {
           var movieRecord = await ProcessMovie(movie, userRecord, requestDebug);

           if (movieRecord == null)
           {
               yield return new ImportMediaResponse
               {
                   Data = $"[Error] - Failed to process movie '{movie.Movie.Title} ({movie.Movie.Year})'.",
                   Total = movies.Count,
                   Processed = processedMovies,
                   Type = MediaType.Movie.ToString()
               };
               continue;
           }

           var lastWatchedAt = _moviesService.GetWatchedMovieByLastWatchedAt(userRecord.Username, movie.Movie.Ids.TMDB, movie.LastWatchedAt);

           if (lastWatchedAt == null)
           {
               var watchingHistory = await _traktProvider.GetWatchedMovieHistory(userRecord.Username, movie.Movie.Ids.Trakt);

               foreach (var watchHistory in watchingHistory)
               {
                   await _moviesService.MarkMovieAsWatched(userRecord, movieRecord, watchHistory.WatchedAt);
               }   
           }

           processedMovies++;
           Console.WriteLine($"[INFO] - Processed {processedMovies}/{movies.Count} movies.");
           yield return new ImportMediaResponse
           {
               Data = $"[INFO] - Processed {processedMovies}/{movies.Count} movies.",
               Total = movies.Count,
               Processed = processedMovies,
               Type = MediaType.Movie.ToString()
           };
        }
    }

    private async Task<MovieRecord?> ProcessMovie(TraktMovieResponse movie, UserRecord userRecord, bool requestDebug)
    {
        var existingMovie = _moviesService.GetMovieByTmdbId(movie.Movie.Ids.TMDB);

        if (existingMovie != null)
            return existingMovie;
            
        var details = await _detailsProvider.GetDetailsForMovie(movie.Movie.Ids.TMDB, requestDebug);

        if (details.IsSuccess == false)
        {
            Console.WriteLine($"[ERROR] - Unsuccessful response from TMDB: ({details.StatusCode}) {details.StatusMessage}.");
            return null;
        }

        var movieRecord = new MovieRecord
        {
            Identifier = Guid.NewGuid(),
            Title = movie.Movie.Title,
            Slug = SlugHelper.GenerateSlugFor(movie.Movie.Title),
            Year = movie.Movie.Year,
            TMDB = movie.Movie.Ids.TMDB,
            Poster = $"https://image.tmdb.org/t/p/w300{details.PosterUrl}",
            Overview = details?.Overview,
        };
        
        var genres = await _moviesService.FindOrCreateGenres(details?.Genres.ConvertAll((genre) => genre.Name) ?? []);

        await _moviesService.ImportMovie(userRecord, movieRecord, genres);

        return movieRecord;
    }

    private async IAsyncEnumerable<ImportMediaResponse> ProcessShows(ImportMediaRequest request, List<TraktShowResponse> shows, UserRecord user, bool requestDebug)
    {
        Console.WriteLine($"[INFO] - Will process {shows.Count} shows.");
        yield return new ImportMediaResponse
        {
            Data = $"[INFO] - Will process {shows.Count} shows."
        };

        var processedShows = 0;
        foreach (var show in shows)
        {
            var details = await _detailsProvider.GetDetailsForShow(show.Show.Ids.TMDB, requestDebug);

            if (details.Identifier == 0)
            {
                Console.WriteLine($"[ERROR] - Failed to find show details ({show.Show.Ids.TMDB}).");
                continue;
            }
            
            await ProcessShow(show, details, user, requestDebug);

            var lastWatchedAt = _showsService.GetWatchedShowByLastWatchedAt(user.Username, show.Show.Ids.TMDB, show.LastWatchedAt);

            if (lastWatchedAt == null)
            {
                var watchingHistory = await _traktProvider.GetWatchedShowHistory(user.Username, show.Show.Ids.Trakt);

                foreach (var watchHistory in watchingHistory)
                {
                    var showRecord = await GetShowRecordByTmdbId(show.Show.Ids.TMDB, details, requestDebug);
                    
                    if (showRecord.Identifier.ToString() == Guid.Empty.ToString())
                    {  
                        Console.WriteLine($"[WARN] - Failed to save watch history. Did not find Show by Show.TMDB: {show.Show.Ids.TMDB}.");
                        continue;
                    }
                    
                    var seasonRecord = await GetSeasonRecordByShowTmdbId(showRecord, watchHistory.Episode.Season);

                    if (seasonRecord.Identifier.ToString() == Guid.Empty.ToString())
                    {
                        Console.WriteLine($"[WARN] - Failed to save watch history. Did not find Season by Show.TMDB: {show.Show.Ids.TMDB}, Season.Number: {watchHistory.Episode.Season}.");
                        continue;
                    }
                    
                    var episodeRecord = await GetEpisodeRecordByShowTmdbId(showRecord, seasonRecord, watchHistory.Episode.Number, requestDebug);

                    if (episodeRecord.Identifier.ToString() == Guid.Empty.ToString())
                    {
                        Console.WriteLine($"[WARN] - Failed to save watch history. Did not find Episode by Show.TMDB: {show.Show.Ids.TMDB}, Season.Number: {watchHistory.Episode.Season}, Episode.Number: {watchHistory.Episode.Number}.");
                        continue;
                    }
                    
                    await _showsService.MarkEpisodeAsWatched(user, showRecord, seasonRecord, episodeRecord, watchHistory.WatchedAt);
                }
            }
                
            processedShows++;
            Console.WriteLine($"[INFO] - Processed {processedShows}/{shows.Count} shows.");
            yield return new ImportMediaResponse
            {
                Data = $"[INFO] - Processed {processedShows}/{shows.Count} shows.",
                Total = shows.Count,
                Processed = processedShows,
                Type = MediaType.Episode.ToString()
            };
        }
    }

    private async Task ProcessShow(TraktShowResponse show, TmdbShowDetails details, UserRecord userRecord, bool requestDebug)
    {   
        var showRecord = await GetShowRecordByTmdbId(show.Show.Ids.TMDB, details, requestDebug);
        
        if(showRecord.TMDB == null)
        {
            Console.WriteLine($"[ERROR] - Show Tmdb not found: Show.Title: '{showRecord.Title}'. {JsonConvert.SerializeObject(show)}");
            return;
        }
        
        if(requestDebug)
            Console.WriteLine($"[DEBUG] - Got Show Record. ShowTmdbId: {show.Show.Ids.TMDB}. {JsonConvert.SerializeObject(show)}");
        
        var genres = await _moviesService.FindOrCreateGenres(details?.Genres.ConvertAll((genre) => genre.Name) ?? []);
        
        _showsService.ImportShow(userRecord, showRecord, genres).Wait();
            
        foreach (var season in show.Seasons)
        {
            var seasonRecord = await GetSeasonRecordByShowTmdbId(showRecord, season.Number);
            
            if(requestDebug)
                Console.WriteLine($"[DEBUG] - Got Season Record. ShowRecordId: {showRecord.Identifier}, SeasonNumber: {season.Number}. {JsonConvert.SerializeObject(season)}");
            
            _showsService.ImportSeason(userRecord, showRecord, seasonRecord).Wait();
                    
            foreach (var episode in season.Episodes)
            {
                var episodeRecord = await GetEpisodeRecordByShowTmdbId(showRecord, seasonRecord, episode.Number, requestDebug);
                
                if(requestDebug)
                    Console.WriteLine($"[DEBUG] - Got Episode Record. ShowRecordId: {showRecord.Identifier}, SeasonIdentifier: {seasonRecord.Identifier}, EpisodeNumber: {episode.Number}. {JsonConvert.SerializeObject(episode)}");
                
                _showsService.ImportEpisode(userRecord, showRecord, seasonRecord, episodeRecord).Wait();
            }
        }
    }

    private async Task<ShowRecord> GetShowRecordByTmdbId(string tmdbId, TmdbShowDetails details, bool requestDebug)
    {
        var showRecord = await _showsService.GetShowByTmdbId(tmdbId);

        if (showRecord != null)
            return showRecord;
        
        showRecord = new ShowRecord
        {
            Identifier = Guid.NewGuid(),
            Title = details.Title,
            Slug = SlugHelper.GenerateSlugFor(details.Title),
            Year = details.FirstAirDate.Year,
            TMDB = tmdbId,
            Poster = $"https://image.tmdb.org/t/p/w300{details.PosterUrl}",
            Overview = details.Overview
        };

        return showRecord;
    }

    private async Task<SeasonRecord> GetSeasonRecordByShowTmdbId(ShowRecord showRecord, int seasonNumber)
    {
        var seasonRecord = await _showsService.GetSeasonBy(seasonNumber, showRecord.Identifier);

        if (seasonRecord != null)
            return seasonRecord;

        seasonRecord = new SeasonRecord
        {
            Identifier = Guid.NewGuid(),
            Number = seasonNumber,
            Title = $"Season {seasonNumber}",
            Show = showRecord
        };
        
        return seasonRecord;
    }

    private async Task<EpisodeRecord> GetEpisodeRecordByShowTmdbId(ShowRecord show, SeasonRecord season, int episodeNumber, bool requestDebug)
    {
        try
        {
            var episodeRecord = await _showsService.GetEpisodeBy(episodeNumber, season.Identifier);

            if (episodeRecord != null)
                return episodeRecord;

            var episodeDetails = await _detailsProvider.GetEpisodeDetails(show.TMDB, season.Number, episodeNumber, requestDebug);

            episodeRecord = new EpisodeRecord
            {
                Identifier = Guid.NewGuid(),
                Number = episodeNumber,
                Title = episodeDetails.Title ?? show.Title,
                Season = season
            };

            return episodeRecord;
        }
        catch (Exception)
        {
            Console.WriteLine($"[ERROR] - Failed getting episode record by show tmdb id. ShowIdentifier: {show}, SeasonIdentifier: {season}, EpisodeNumer: {episodeNumber}");
            throw;
        }
    }

    public async Task<List<string>> GetPosters()
    {
        return await _overseerrService.GetPosters();
    }
}