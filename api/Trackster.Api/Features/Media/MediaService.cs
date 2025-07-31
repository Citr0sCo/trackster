using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Media.Importers.TraktImporter;
using Trackster.Api.Features.Media.Types;

namespace Trackster.Api.Features.Media;

public class MediaService
{
    private readonly IMediaRepository _mediaRepository;
    private readonly TraktImportProvider _provider;
    private readonly TmdbImportProvider _detailsProvider;
    private readonly WatchingNowService _watchingNowService;
    
    private const string MOVIE_MEDIA_TYPE = "movie";
    private string EPISODE_MEDIA_TYPE = "episode";

    public MediaService(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
        _provider = new TraktImportProvider();
        _detailsProvider = new TmdbImportProvider();
        _watchingNowService = WatchingNowService.Instance();
    }

    public async Task<ImportMediaResponse> ImportMedia(ImportMediaRequest request)
    {
        if (request.Type == ImportType.Trakt && request.Username != null)
        {
            var movies = await _provider.GetMovies(request.Username);
            var shows = await _provider.GetShows(request.Username);

            await _mediaRepository.ImportMovies(request.Username, movies);
            await _mediaRepository.ImportShows(request.Username,shows);
        }
        
        return new ImportMediaResponse();
    }

    public GetAllMoviesResponse GetAllMovies(string username)
    {
        var movies = _mediaRepository.GetAllMovies(username);

        return new GetAllMoviesResponse
        {
            Movies = movies
        };
    }

    public GetAllShowsResponse GetAllShows(string username)
    {
        var shows = _mediaRepository.GetAllShows(username);

        return new GetAllShowsResponse
        {
            Shows = shows
        };
    }

    public GetHistoryForUserResponse GetHistoryForUser(string username)
    {
        var movies = _mediaRepository.GetAllMovies(username);
        var shows = _mediaRepository.GetAllShows(username);

        var media = new List<Types.Media>();
        
        foreach (var movie in movies)
        {
            media.Add(new  Types.Media
            {
                Identifier = movie.Identifier,
                Title = movie.Title,
                Year = movie.Year,
                Overview = movie.Overview,
                Poster = movie.Poster,
                TMDB = movie.TMDB,
                Type = MediaType.Movie,
                WatchedAt = movie.WatchedAt
            });
        }
        
        foreach (var show in shows)
        {
            media.Add(new  Types.Media
            {
                Identifier = show.Identifier,
                Title = show.Title,
                ParentTitle = show.ParentTitle,
                GrandParentTitle = show.GrandParentTitle,
                Year = show.Year,
                Overview = show.Overview,
                Poster = show.Poster,
                TMDB = show.TMDB,
                Type = MediaType.Show,
                SeasonNumber = show.SeasonNumber,
                EpisodeNumber = show.EpisodeNumber,
                WatchedAt = show.WatchedAt
            });
        }
        
        return new GetHistoryForUserResponse
        {
            Media = media.OrderByDescending(x => x.WatchedAt).ToList()
        };
    }

    public GetMediaByIdResponse? GetMediaByIdentifier(Guid identifier)
    {
        var movie = _mediaRepository.GetMovieByIdentifier(identifier);

        if (movie != null)
        {
            return new GetMediaByIdResponse
            {
                Media = new Types.Media
                {
                    Identifier = movie.Identifier,
                    Title = movie.Title,
                    Year = movie.Year,
                    Overview = movie.Overview,
                    Poster = movie.Poster,
                    TMDB = movie.TMDB,
                    Type = MediaType.Movie,
                    WatchedAt = movie.WatchedAt
                }
            };
        }
        
        var show = _mediaRepository.GetShowByIdentifier(identifier);

        if (show != null)
        {
            return new GetMediaByIdResponse
            {
                Media = new Types.Media
                {
                    Identifier = show.Identifier,
                    Title = show.Title,
                    ParentTitle = show.ParentTitle,
                    GrandParentTitle = show.GrandParentTitle,
                    Year = show.Year,
                    Overview = show.Overview,
                    Poster = show.Poster,
                    TMDB = show.TMDB,
                    Type = MediaType.Show,
                    WatchedAt = show.WatchedAt,
                    SeasonNumber = show.SeasonNumber,
                    EpisodeNumber = show.EpisodeNumber,
                }
            };
        }
        
        return null;
    }

    public async Task MarkMediaAsWatched(string mediaType, int year, string title, string? parentTitle = null, int seasonNumber = 0)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
            await MarkMovieAsWatched(title, year);
        
        if (mediaType == EPISODE_MEDIA_TYPE)
            await MarkEpisodeAsWatched(parentTitle!, title, year, seasonNumber);
    }

    public async Task MarkMovieAsWatched(string title, int year)
    {
        var movie = await SearchForMovieBy(title, year);
        _mediaRepository.ImportMovie("citr0s", movie);
    }

    public async Task MarkEpisodeAsWatched(string showTitle, string episodeTitle, int year, int seasonNumber)
    {
        var episode = await SearchForEpisode(showTitle, episodeTitle, year, seasonNumber);
        _mediaRepository.ImportEpisode("citr0s", episode.Season.Show, episode.Season, episode);
    }
    
    public async Task<MovieRecord> SearchForMovieBy(string title, int year)
    {
        var searchResults = await _detailsProvider.FindMovieByTitleAndYear(title, year);
        var tmdbReference = searchResults.Results.FirstOrDefault()?.Id.ToString();
        var movie = await _detailsProvider.GetDetailsForMovie(tmdbReference ?? "");
        
        return new MovieRecord
        {
            Identifier = Guid.NewGuid(),
            Title = title,
            TMDB = tmdbReference,
            Year = year,
            Overview = movie.Overview,
            Poster = $"https://image.tmdb.org/t/p/w185{movie.PosterUrl}"
        };
    }
    
    public async Task<EpisodeRecord> SearchForEpisode(string showTitle, string episodeTitle, int year, int seasonNumber)
    {
        var searchResults = await _detailsProvider.FindShowByTitleAndYear(showTitle, year);
        var tmdbReference = searchResults.Results.FirstOrDefault()?.Id.ToString();
        var parsedShow = await _detailsProvider.GetDetailsForShow(tmdbReference ?? "");
        var parsedSeason = await _detailsProvider.GetDetailsForSeason(parsedShow.Identifier, seasonNumber);
        var parsedEpisode = parsedSeason.Episodes.FirstOrDefault(x => x.Title.ToLower() == episodeTitle.ToLower());
        
        var show = new ShowRecord
        {
            Identifier = Guid.NewGuid(),
            Title = parsedShow.Title,
            Overview = parsedShow.Overview,
            Poster = $"https://image.tmdb.org/t/p/w185{parsedShow.PosterUrl}",
            TMDB = parsedShow.Identifier.ToString(),
            Year = parsedShow.FirstAirDate.Year
        };

        var season = new SeasonRecord
        {
            Identifier = Guid.NewGuid(),
            Show = show,
            Number = seasonNumber,
            Title = parsedSeason.Title
        };

        return new EpisodeRecord
        {
            Identifier = Guid.NewGuid(),
            Season = season,
            Number = parsedEpisode.EpisodeNumber,
            Title = parsedEpisode.Title,
        };
    }

    public async void MarkMediaAsWatchingNow(string mediaType, int year, string title, string grandParentTitle, int seasonNumber, int watchedAmountInMilliseconds, int duration)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
        {
            var movie = await SearchForMovieBy(title, year);
            _watchingNowService.MarkAsWatchingMovie("citr0s", movie, watchedAmountInMilliseconds, duration);
        }
        
        if (mediaType == EPISODE_MEDIA_TYPE)
        {
            var episode = await SearchForEpisode(grandParentTitle,  title, year, seasonNumber);
            _watchingNowService.MarkAsWatchingEpisode("citr0s", episode, watchedAmountInMilliseconds, duration);
        }
            
        Console.WriteLine($"Marking a media as watching now. {title}, {grandParentTitle}, {seasonNumber}, {year}.");
    }

    public async void RemoveMediaAsWatchingNow(string mediaType, int year, string title, string grandParentTitle, int seasonNumber)
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

    public async void PauseMediaAsWatchingNow(string mediaType, int year, string title, string grandParentTitle, int seasonNumber, int watchedAmountInMilliseconds, int duration)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
        {
            var movie = await SearchForMovieBy(title, year);
            _watchingNowService.MarkAsPausedWatchingMovie("citr0s", movie, watchedAmountInMilliseconds, duration);
        }

        if (mediaType == EPISODE_MEDIA_TYPE)
        {
            var episode = await SearchForEpisode(grandParentTitle,  title, year, seasonNumber);
            _watchingNowService.MarkAsPausedWatchingEpisode("citr0s", episode, watchedAmountInMilliseconds, duration);
        }
            
        Console.WriteLine($"Marking a media as paused watching. {title}, {grandParentTitle}, {seasonNumber}, {year}.");
    }
}