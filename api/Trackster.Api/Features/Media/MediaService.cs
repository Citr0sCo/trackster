using Castle.Components.DictionaryAdapter.Xml;
using Trackster.Api.Core.Helpers;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Media.Importers.TraktImporter;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.Movies;
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
    }

    public async Task<ImportMediaResponse> ImportMedia(ImportMediaRequest request)
    {
        if (request.Type == ImportType.Trakt && request.Username != null)
        {
            var movies = await _traktProvider.GetMovies(request.Username);
            var shows = await _traktProvider.GetShows(request.Username);

            var value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (value == "Development")
            {
                movies = movies.Take(10).ToList();
                shows = shows.Take(10).ToList();
            }

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

            foreach (var movie in movies)
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

                var watchingHistory = await _traktProvider.GetWatchedMovieHistory(request.Username, movie.Movie.Ids.Trakt);

                foreach (var watchHistory in watchingHistory)
                {
                    await _moviesService.MarkMovieAsWatched(request.Username, movie.Movie.Ids.TMDB, watchHistory.WatchedAt);
                }
            }

            foreach (var show in shows)
            {
                var showRecord = await _showsService.GetShowByTmdbId(show.Show.Ids.TMDB);
                var details = await _detailsProvider.GetDetailsForShow(show.Show.Ids.TMDB);

                if (showRecord == null)
                {
                    showRecord = new ShowRecord
                    {
                        Identifier = Guid.NewGuid(),
                        Title = show.Show.Title,
                        Slug = SlugHelper.GenerateSlugFor(show.Show.Title),
                        Year = show.Show.Year,
                        TMDB = show.Show.Ids.TMDB,
                        Poster = $"https://image.tmdb.org/t/p/w300{details?.PosterUrl}",
                        Overview = details?.Overview,
                    };
                    
                    await _showsService.ImportShow(userRecord, showRecord);
                }
                
                foreach (var season in show.Seasons)
                {
                    var seasonRecord = await _showsService.GetSeasonBy(season.Number, showRecord.Identifier);

                    if (seasonRecord == null)
                    {
                        var title = $"Season {season.Number}";

                        if (season.Number > 0 && details?.Seasons.Count > (season.Number - 1))
                            title = details.Seasons[season.Number - 1].Title;

                        seasonRecord = new SeasonRecord
                        {
                            Identifier = Guid.NewGuid(),
                            Number = season.Number,
                            Title = title,
                            Show = showRecord
                        };
                        
                        await _showsService.ImportSeason(userRecord, showRecord, seasonRecord);
                        
                        foreach (var episode in season.Episodes)
                        {
                            var episodeRecord = await _showsService.GetEpisodeBy(episode.Number, seasonRecord.Identifier);
                            
                            if (episodeRecord == null)
                            {
                                var episodeDetails = await _detailsProvider.GetEpisodeDetails(show.Show.Ids.TMDB, season.Number, episode.Number);
                                
                                episodeRecord = new EpisodeRecord
                                {
                                    Identifier = Guid.NewGuid(),
                                    Number = episode.Number,
                                    Title = episodeDetails.Title ?? show.Show.Title,
                                    Season = seasonRecord
                                };

                                await _showsService.ImportEpisode(userRecord, showRecord, seasonRecord, episodeRecord);
                            }          
                        }
                    }
                }
                    
                var watchingHistory = await _traktProvider.GetWatchedShowHistory(request.Username, show.Show.Ids.Trakt);

                foreach (var watchHistory in watchingHistory)
                {
                    await _showsService.MarkEpisodeAsWatched(request.Username, show.Show.Ids.TMDB, watchHistory.Episode.Season, watchHistory.Episode.Number, watchHistory.WatchedAt);
                }
            }
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
                Type = MediaType.Show.ToString(),
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

    public async Task MarkMediaAsWatched(string mediaType, int year, string title, string? parentTitle = null,
        int seasonNumber = 0)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
            await MarkMovieAsWatched(title, year);

        if (mediaType == EPISODE_MEDIA_TYPE)
            await MarkEpisodeAsWatched(parentTitle!, title, year, seasonNumber);
    }

    public async void MarkMediaAsWatchingNow(string mediaType, int year, string title, string grandParentTitle,
        int seasonNumber, int watchedAmountInMilliseconds, int duration)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
        {
            var movie = await _moviesService.SearchForMovieBy(title, year);
            _watchingNowService.MarkAsWatchingMovie("citr0s", movie, watchedAmountInMilliseconds, duration);
        }

        if (mediaType == EPISODE_MEDIA_TYPE)
        {
            var episode = await _showsService.SearchForEpisode(grandParentTitle, title, year, seasonNumber);
            _watchingNowService.MarkAsWatchingEpisode("citr0s", episode, watchedAmountInMilliseconds, duration);
        }

        Console.WriteLine($"Marking a media as watching now. {title}, {grandParentTitle}, {seasonNumber}, {year}.");
    }

    public async void RemoveMediaAsWatchingNow(string mediaType, int year, string title, string grandParentTitle,
        int seasonNumber)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
        {
            _watchingNowService.MarkAsStoppedWatchingMovie("citr0s");
        }

        if (mediaType == EPISODE_MEDIA_TYPE)
        {
            _watchingNowService.MarkAsStoppedWatchingEpisode("citr0s");
        }

        Console.WriteLine($"Marking a media as stopped watching. {title}, {grandParentTitle}, {seasonNumber}, {year}.");
    }

    public async void PauseMediaAsWatchingNow(string mediaType, int year, string title, string grandParentTitle,
        int seasonNumber, int watchedAmountInMilliseconds, int duration)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
        {
            var movie = await _moviesService.SearchForMovieBy(title, year);
            _watchingNowService.MarkAsPausedWatchingMovie("citr0s", movie, watchedAmountInMilliseconds, duration);
        }

        if (mediaType == EPISODE_MEDIA_TYPE)
        {
            var episode = await _showsService.SearchForEpisode(grandParentTitle, title, year, seasonNumber);
            _watchingNowService.MarkAsPausedWatchingEpisode("citr0s", episode, watchedAmountInMilliseconds, duration);
        }

        Console.WriteLine($"Marking a media as paused watching. {title}, {grandParentTitle}, {seasonNumber}, {year}.");
    }

    private async Task MarkMovieAsWatched(string title, int year)
    {
        var user = await _usersService.GetUserByUsername("citr0s");
        var movie = await _moviesService.SearchForMovieBy(title, year);
        await _moviesService.MarkMovieAsWatched(user.Username, movie.TMDB, DateTime.UtcNow);
    }

    private async Task MarkEpisodeAsWatched(string showTitle, string episodeTitle, int year, int seasonNumber)
    {
        var user = await _usersService.GetUserByUsername("citr0s");
        var episode = await _showsService.SearchForEpisode(showTitle, episodeTitle, year, seasonNumber);
        await _showsService.MarkEpisodeAsWatched(user.Username, episode.Season.Show.TMDB, episode.Season.Number, episode.Number, DateTime.Now);
    }
}