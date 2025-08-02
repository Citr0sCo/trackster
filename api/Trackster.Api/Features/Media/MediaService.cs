using Trackster.Api.Features.Media.Importers.TraktImporter;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.Movies;
using Trackster.Api.Features.Shows;

namespace Trackster.Api.Features.Media;

public class MediaService
{
    private readonly IMoviesService _moviesService;
    private readonly IShowsService _showsService;
    private readonly TraktImportProvider _provider;
    private readonly WatchingNowService _watchingNowService;
    
    private const string MOVIE_MEDIA_TYPE = "movie";
    private string EPISODE_MEDIA_TYPE = "episode";

    public MediaService(IMoviesService moviesService, IShowsService showsService)
    {
        _moviesService = moviesService;
        _showsService = showsService;
        _provider = new TraktImportProvider();
        _watchingNowService = WatchingNowService.Instance();
    }

    public async Task<ImportMediaResponse> ImportMedia(ImportMediaRequest request)
    {
        if (request.Type == ImportType.Trakt && request.Username != null)
        {
            var movies = await _provider.GetMovies(request.Username);
            var shows = await _provider.GetShows(request.Username);

            var value = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            
            if(value == "Development")
            {
                movies = movies.Take(10).ToList();
                shows = shows.Take(10).ToList();
            }
            
            await _moviesService.ImportMovies(request.Username, movies);
            await _showsService.ImportShows(request.Username,shows);
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
            media.Add(new  Types.Media
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
    
    public async Task MarkMediaAsWatched(string mediaType, int year, string title, string? parentTitle = null, int seasonNumber = 0)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
            await MarkMovieAsWatched(title, year);
        
        if (mediaType == EPISODE_MEDIA_TYPE)
            await MarkEpisodeAsWatched(parentTitle!, title, year, seasonNumber);
    }
    
    public async void MarkMediaAsWatchingNow(string mediaType, int year, string title, string grandParentTitle, int seasonNumber, int watchedAmountInMilliseconds, int duration)
    {
        if (mediaType == MOVIE_MEDIA_TYPE)
        {
            var movie = await _moviesService.SearchForMovieBy(title, year);
            _watchingNowService.MarkAsWatchingMovie("citr0s", movie, watchedAmountInMilliseconds, duration);
        }
        
        if (mediaType == EPISODE_MEDIA_TYPE)
        {
            var episode = await _showsService.SearchForEpisode(grandParentTitle,  title, year, seasonNumber);
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
            var movie = await _moviesService.SearchForMovieBy(title, year);
            _watchingNowService.MarkAsPausedWatchingMovie("citr0s", movie, watchedAmountInMilliseconds, duration);
        }

        if (mediaType == EPISODE_MEDIA_TYPE)
        {
            var episode = await _showsService.SearchForEpisode(grandParentTitle,  title, year, seasonNumber);
            _watchingNowService.MarkAsPausedWatchingEpisode("citr0s", episode, watchedAmountInMilliseconds, duration);
        }
            
        Console.WriteLine($"Marking a media as paused watching. {title}, {grandParentTitle}, {seasonNumber}, {year}.");
    }

    private async Task MarkMovieAsWatched(string title, int year)
    {
        var movie = await _moviesService.SearchForMovieBy(title, year);
        _moviesService.ImportMovie("citr0s", movie);
    }

    private async Task MarkEpisodeAsWatched(string showTitle, string episodeTitle, int year, int seasonNumber)
    {
        var episode = await _showsService.SearchForEpisode(showTitle, episodeTitle, year, seasonNumber);
        _showsService.ImportEpisode("citr0s", episode.Season.Show, episode.Season, episode);
    }
}