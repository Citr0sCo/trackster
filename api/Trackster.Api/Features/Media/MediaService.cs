using System.Diagnostics;
using Newtonsoft.Json;
using Trackster.Api.Core.Helpers;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Media.Importers.TraktImporter;
using Trackster.Api.Features.Media.Importers.TraktImporter.Types;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.Movies;
using Trackster.Api.Features.Movies.Types;
using Trackster.Api.Features.Notifications;
using Trackster.Api.Features.Shows;
using Trackster.Api.Features.Users;

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

    private const string MOVIE_MEDIA_TYPE = "movie";
    private string EPISODE_MEDIA_TYPE = "episode";

    public MediaService(IMoviesService moviesService, IShowsService showsService, IUsersService usersService)
    {
        _moviesService = moviesService;
        _showsService = showsService;
        _usersService = usersService;
        _traktProvider = new TraktImportProvider();
        _watchingNowService = WatchingNowService.Instance();
        _detailsProvider = new TmdbImportProvider();
        _notificationsService = new NotificationsService();
    }

    public async Task<ImportMediaResponse> ImportMedia(ImportMediaRequest request)
    {
        if (request.Type == ImportType.Trakt && request.Username != null)
        {
            var stopwatch = Stopwatch.StartNew();
            
            Console.WriteLine($"[INFO] - Starting Trakt import...");
            
            var movies = await _traktProvider.GetMovies(request.Username);
            var shows = await _traktProvider.GetShows(request.Username);

            var value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (value == "Development")
            {
                movies = movies.OrderByDescending(x => x.LastWatchedAt).Take(25).ToList();
                shows = shows.OrderByDescending(x => x.LastWatchedAt).Take(25).ToList();
            }

            var user = await ProcessUser(request);

            await ProcessMovies(request, movies, user);

            await ProcessShows(request, shows, user);
            
            stopwatch.Stop();
            
            var time = TimeSpan.FromMilliseconds(stopwatch.ElapsedMilliseconds);
            Console.WriteLine($"[INFO] - Finished Trakt import in {time.Minutes}m {time.Seconds}s!");
        }

        return new ImportMediaResponse();
    }

    public GetHistoryForUserResponse GetHistoryForUser(string username, int results, int page)
    {
        var movies = _moviesService.GetAllWatchedMovies(username, results, page);
        var shows = _showsService.GetAllWatchedShows(username, results, page);

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

        foreach (var show in shows.WatchedShows)
        {
            media.Add(new Types.Media
            {
                Identifier = show.Show.Identifier,
                Title = show.Episode.Title,
                ParentTitle = show.Season.Title,
                GrandParentTitle = show.Show.Title,
                Slug = show.Show.Slug,
                Year = show.Show.Year,
                Overview = show.Show.Overview,
                Poster = show.Show.Poster,
                TMDB = show.Show.TMDB,
                Type = MediaType.Episode.ToString(),
                SeasonNumber = show.Season.Number,
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
        var shows = _showsService.GetAllWatchedShows(username, 10000, 1);

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

        foreach (var show in shows.WatchedShows)
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
        var shows = _showsService.GetAllWatchedShows(username, 10000, 1);

        return new GetStatsResponse
        {
            Total = movies.WatchedMovies.Count + shows.WatchedShows.Count,
            MoviesWatched = movies.WatchedMovies.Count,
            EpisodesWatched = shows.WatchedShows.Count
        };
    }

    public async Task MarkMediaAsWatched(string mediaType, int year, string title, string? parentTitle = null, string? grandParentTitle = null, int seasonNumber = 0)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
            await MarkMovieAsWatched(title, year);

        if (mediaType == EPISODE_MEDIA_TYPE)
            await MarkEpisodeAsWatched(grandParentTitle!, parentTitle!, title, year, seasonNumber);
    }

    public async void MarkMediaAsWatchingNow(string mediaType, int year, string title, string parentTitle, string grandParentTitle, int seasonNumber, int watchedAmountInMilliseconds, int duration)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
        {
            var movie = await _moviesService.SearchForMovieBy(title, year);
            _watchingNowService.MarkAsWatchingMovie("citr0s", movie, watchedAmountInMilliseconds, duration);
        }

        if (mediaType == EPISODE_MEDIA_TYPE)
        {
            var episode = await _showsService.SearchForEpisode(grandParentTitle, parentTitle, title, year, seasonNumber);
            _watchingNowService.MarkAsWatchingEpisode("citr0s", episode, watchedAmountInMilliseconds, duration);
        }

        Console.WriteLine($"Marking a media as watching now. {title}, {grandParentTitle}, {seasonNumber}, {year}.");
    }

    public async void RemoveMediaAsWatchingNow(string mediaType, int year, string title, string parentTitle, string grandParentTitle, int seasonNumber)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
            _watchingNowService.MarkAsStoppedWatchingMovie("citr0s");

        if (mediaType == EPISODE_MEDIA_TYPE)
            _watchingNowService.MarkAsStoppedWatchingEpisode("citr0s");

        Console.WriteLine($"Marking a media as stopped watching. {title}, {grandParentTitle}, {parentTitle}, {seasonNumber}, {year}.");
    }

    public async void PauseMediaAsWatchingNow(string mediaType, int year, string title, string parentTitle, string grandParentTitle, int seasonNumber, int watchedAmountInMilliseconds, int duration)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
        {
            var movie = await _moviesService.SearchForMovieBy(title, year);
            _watchingNowService.MarkAsPausedWatchingMovie("citr0s", movie, watchedAmountInMilliseconds, duration);
        }

        if (mediaType == EPISODE_MEDIA_TYPE)
        {
            var episode = await _showsService.SearchForEpisode(grandParentTitle, parentTitle, title, year, seasonNumber);
            _watchingNowService.MarkAsPausedWatchingEpisode("citr0s", episode, watchedAmountInMilliseconds, duration);
        }

        Console.WriteLine($"Marking a media as paused watching. {title}, {grandParentTitle}, {seasonNumber}, {year}.");
    }

    private async Task MarkMovieAsWatched(string title, int year)
    {
        try
        {
            var user = await _usersService.GetUserByUsername("citr0s");
            var movie = await _moviesService.SearchForMovieBy(title, year);
            await _moviesService.MarkMovieAsWatched(user, movie, DateTime.UtcNow);
            _notificationsService.Send($"Movie '{title} ({year})' marked as watched.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL] - Failed to mark movie as watched. Exception: {ex.Message}.");
            Console.WriteLine(ex);
        }
    }

    private async Task MarkEpisodeAsWatched(string showTitle, string seasonTitle, string episodeTitle, int year, int seasonNumber)
    {
        try
        {
            var user = await _usersService.GetUserByUsername("citr0s");
            var episode = await _showsService.SearchForEpisode(showTitle, seasonTitle, episodeTitle, year, seasonNumber);
            await _showsService.MarkEpisodeAsWatched(user, episode.Season.Show, episode.Season, episode, DateTime.Now);
            _notificationsService.Send($"Episode '{episodeTitle}' of show '{showTitle}' marked as watched.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[FATAL] - Failed to mark episode as watched. Exception: {ex.Message}.");
            Console.WriteLine(ex);
        }
    }

    private async Task<UserRecord> ProcessUser(ImportMediaRequest request)
    {
        var userRecord = await _usersService.GetUserByUsername(request.Username);

        if (userRecord == null)
        {
            userRecord = new UserRecord
            {
                Identifier = Guid.NewGuid(),
                Username = request.Username,
            };

            await _usersService.CreateUser(userRecord);
        }

        return userRecord;
    }
    
    private async Task ProcessMovies(ImportMediaRequest request, List<TraktMovieResponse> movies, UserRecord userRecord)
    {
        Console.WriteLine($"[INFO] - Will process {movies.Count} movies.");

        var processedMovies = 0;
        foreach (var movie in movies)
        {
           ProcessMovie(movie, userRecord).Wait();

           var lastWatchedAt = _moviesService.GetWatchedMovieByLastWatchedAt(userRecord.Username, movie.Movie.Ids.TMDB, movie.LastWatchedAt);

           if (lastWatchedAt == null)
           {
               var watchingHistory = await _traktProvider.GetWatchedMovieHistory(userRecord.Username, movie.Movie.Ids.Trakt);

               foreach (var watchHistory in watchingHistory)
               {
                   var movieRecord = await GetMovieRecordByTmdbId(movie.Movie.Ids.TMDB);
                   await _moviesService.MarkMovieAsWatched(userRecord, movieRecord, watchHistory.WatchedAt);
               }   
           }

           processedMovies++;
           Console.WriteLine($"[INFO] - Processed {processedMovies}/{movies.Count} movies.");
        }
    }

    private async Task ProcessMovie(TraktMovieResponse movie, UserRecord userRecord)
    {
        var existingMovie = _moviesService.GetMovieByTmdbId(movie.Movie.Ids.TMDB);

        if (existingMovie == null)
        {
            var details = await _detailsProvider.GetDetailsForMovie(movie.Movie.Ids.TMDB);

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

            await _moviesService.ImportMovie(userRecord, movieRecord);
        }
    }

    private async Task ProcessShows(ImportMediaRequest request, List<TraktShowResponse> shows, UserRecord user)
    {
        Console.WriteLine($"[INFO] - Will process {shows.Count} shows.");

        var processedShows = 0;
        foreach (var show in shows)
        {
            await ProcessShow(show, user);

            var lastWatchedAt = _showsService.GetWatchedShowByLastWatchedAt(user.Username, show.Show.Ids.TMDB, show.LastWatchedAt);

            if (lastWatchedAt == null)
            {
                var watchingHistory = await _traktProvider.GetWatchedShowHistory(user.Username, show.Show.Ids.Trakt);

                foreach (var watchHistory in watchingHistory)
                {
                    var showRecord = await GetShowRecordByTmdbId(show.Show.Ids.TMDB);
                    var seasonRecord = await GetSeasonRecordByShowTmdbId(showRecord.Identifier, watchHistory.Episode.Season);
                    var episodeRecord = await GetEpisodeRecordByShowTmdbId(showRecord.Identifier, seasonRecord.Identifier,  watchHistory.Episode.Number);
                    
                    await _showsService.MarkEpisodeAsWatched(user, showRecord, seasonRecord, episodeRecord, watchHistory.WatchedAt);
                }
            }
                
            processedShows++;
            Console.WriteLine($"[INFO] - Processed {processedShows}/{shows.Count} shows.");
        }
    }

    private async Task ProcessShow(TraktShowResponse show, UserRecord userRecord)
    {
        var showRecord = await GetShowRecordByTmdbId(show.Show.Ids.TMDB);
        Console.WriteLine($"[DEBUG] - Got Show Record. ShowTbdbId: {show.Show.Ids.TMDB}. {JsonConvert.SerializeObject(show)}");
        _showsService.ImportShow(userRecord, showRecord).Wait();
            
        foreach (var season in show.Seasons)
        {
            var seasonRecord = await GetSeasonRecordByShowTmdbId(showRecord.Identifier, season.Number);
            Console.WriteLine($"[DEBUG] - Got Season Record. ShowRecordId: {showRecord.Identifier}, SeasonNumber: {season.Number}. {JsonConvert.SerializeObject(season)}");
            _showsService.ImportSeason(userRecord, showRecord, seasonRecord).Wait();
                    
            foreach (var episode in season.Episodes)
            {
                var episodeRecord = await GetEpisodeRecordByShowTmdbId(showRecord.Identifier, seasonRecord.Identifier,  episode.Number);
                Console.WriteLine($"[DEBUG] - Got Episode Record. ShowRecordId: {showRecord.Identifier}, SeasonIdentifier: {seasonRecord.Identifier}, EpisodeNumber: {episode.Number}. {JsonConvert.SerializeObject(episode)}");
                _showsService.ImportEpisode(userRecord, showRecord, seasonRecord, episodeRecord).Wait();
            }
        }
    }

    private async Task<MovieRecord> GetMovieRecordByTmdbId(string tmdbId)
    {
        var movieRecord = _moviesService.GetMovieByTmdbId(tmdbId);

        if (movieRecord != null)
            return movieRecord;
        
        var details = await _detailsProvider.GetDetailsForMovie(tmdbId);

        movieRecord = new MovieRecord
        {
            Identifier = Guid.NewGuid(),
            Title = details.Title,
            Slug = SlugHelper.GenerateSlugFor(details.Title),
            Year = details.ReleaseDate.Year,
            TMDB = tmdbId,
            Poster = $"https://image.tmdb.org/t/p/w300{details?.PosterUrl}",
            Overview = details?.Overview,
        };

        return movieRecord;
    }

    private async Task<ShowRecord> GetShowRecordByTmdbId(string tmdbId)
    {
        var showRecord = await _showsService.GetShowByTmdbId(tmdbId);

        if (showRecord != null)
            return showRecord;
        
        var details = await _detailsProvider.GetDetailsForShow(tmdbId);

        showRecord = new ShowRecord
        {
            Identifier = Guid.NewGuid(),
            Title = details.Title,
            Slug = SlugHelper.GenerateSlugFor(details.Title),
            Year = details.FirstAirDate.Year,
            TMDB = tmdbId,
            Poster = $"https://image.tmdb.org/t/p/w300{details?.PosterUrl}",
            Overview = details?.Overview,
        };

        return showRecord;
    }

    private async Task<SeasonRecord> GetSeasonRecordByShowTmdbId(Guid showIdentifier, int seasonNumber)
    {
        var showRecord = _showsService.GetShowByReference(showIdentifier);
        
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

    private async Task<EpisodeRecord> GetEpisodeRecordByShowTmdbId(Guid showIdentifier, Guid seasonIdentifier, int episodeNumber)
    {
        try
        {
            var showRecord = _showsService.GetShowByReference(showIdentifier);
            Console.WriteLine($"[DEBUG] - Got Show Record. ShowIdentifier: {showIdentifier}. {JsonConvert.SerializeObject(showRecord)}");

            var seasonRecord = _showsService.GetSeasonByReference(seasonIdentifier);
            Console.WriteLine($"[DEBUG] - Got Season Record. SeasonIdentifier: {seasonRecord}. {JsonConvert.SerializeObject(seasonRecord)}");
            
            var episodeRecord = await _showsService.GetEpisodeBy(episodeNumber, seasonRecord.Identifier);
            Console.WriteLine($"[DEBUG] - Got Episode Record. EpisodeNumber: {episodeNumber}. {JsonConvert.SerializeObject(episodeNumber)}");

            if (episodeRecord != null)
                return episodeRecord;

            var episodeDetails =
                await _detailsProvider.GetEpisodeDetails(showRecord.TMDB, seasonRecord.Number, episodeNumber);

            episodeRecord = new EpisodeRecord
            {
                Identifier = Guid.NewGuid(),
                Number = episodeNumber,
                Title = episodeDetails.Title ?? showRecord.Title,
                Season = seasonRecord
            };

            return episodeRecord;
        }
        catch (Exception)
        {
            Console.WriteLine($"[ERROR] - Failed getting episode record by show tmdb id. ShowIdentifier: {showIdentifier}, SeasonIdentifier: {seasonIdentifier}, EpisodeNumer: {episodeNumber}");
            throw;
        }
    }
}