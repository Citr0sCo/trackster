using Newtonsoft.Json;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Importers.TmdbImporter;
using Trackster.Api.Features.Media.Importers.TraktImporter;
using Trackster.Api.Features.Media.Types;

namespace Trackster.Api.Features.Media;

public class MediaService
{
    private readonly IMediaRepository _mediaRepository;
    private TraktImportProvider _provider;
    private TmdbImportProvider _detailsProvider;

    public MediaService(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
        _provider = new TraktImportProvider();
        _detailsProvider = new TmdbImportProvider();
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
                WatchedAt = show.WatchedAt
            });
        }
        
        return new GetHistoryForUserResponse
        {
            Media = media.OrderByDescending(x => x.WatchedAt).ToList()
        };
    }

    public async Task MarkMovieAsWatched(string title, int year)
    {
        Console.WriteLine($"Marking {title} as watched by {year}.");
        
        var searchResults = await _detailsProvider.FindMovieByTitleAndYear(title, year);
        var tmdbReference = searchResults.Results.FirstOrDefault()?.Id.ToString();
        var movie = await _detailsProvider.GetDetailsForMovie(tmdbReference ?? "");
        
        _mediaRepository.ImportMovie("citr0s", new MovieRecord
        {
            Identifier = Guid.NewGuid(),
            Title = title,
            TMDB = tmdbReference,
            Year = year,
            Overview = movie.Overview,
            Poster = $"https://image.tmdb.org/t/p/w185{movie.PosterUrl}"
        });
        
        Console.WriteLine($"Marked {title} as watched by {year}.");
    }

    public async Task MarkEpisodeAsWatched(string showTitle, string episodeTitle, int year, int seasonNumber)
    {
        Console.WriteLine($"[DEBUG] - 1/7 - Marking {showTitle} episode {episodeTitle} as watched by {year} season number {seasonNumber}.");
        
        var searchResults = await _detailsProvider.FindShowByTitleAndYear(showTitle, year);
        
        Console.WriteLine($"[DEBUG] - 2/7 - Found show {JsonConvert.SerializeObject(searchResults)}.");
        
        var tmdbReference = searchResults.Results.FirstOrDefault()?.Id.ToString();
        
        Console.WriteLine($"[DEBUG] - 3/7 - Found info for show {showTitle} episode {episodeTitle} reference {tmdbReference}.");
        
        var parsedShow = await _detailsProvider.GetDetailsForShow(tmdbReference ?? "");
        
        Console.WriteLine($"[DEBUG] - 4/7 - Found details for show {parsedShow.Title}.");
        
        var parsedSeason = await _detailsProvider.GetDetailsForSeason(parsedShow.Identifier, seasonNumber);
        
        Console.WriteLine($"[DEBUG] - 5/7 - Found details for season {JsonConvert.SerializeObject(parsedSeason.Episodes)}.");
        
        var parsedEpisode = parsedSeason.Episodes.FirstOrDefault(x => x.Title.ToLower() == episodeTitle.ToLower());
        
        Console.WriteLine($"[DEBUG] - 6/7 - Retrieved info for episode {parsedEpisode.Title}.");

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

        var episode = new EpisodeRecord
        {
            Identifier = Guid.NewGuid(),
            Season = season,
            Number = parsedEpisode.EpisodeNumber,
            Title = parsedEpisode.Title
        };
        
        _mediaRepository.ImportEpisode("citr0s", show, season, episode);
        
        Console.WriteLine($"[DEBUG] - 7/7 - Marked {showTitle} episode {episodeTitle} as watched by {year} season number {seasonNumber}.");
    }

    public void MarkMediaAsWatchingNow(string episodeTitle, string seasonTitle, string showTitle, int year)
    {
        Console.WriteLine($"Marking a media as watching now. {episodeTitle}, {seasonTitle}, {showTitle}, {year}.");
    }

    public void RemoveMediaAsWatchingNow(string episodeTitle, string seasonTitle, string showTitle, int year)
    {
        Console.WriteLine($"Marking a media as stopped watching. {episodeTitle}, {seasonTitle}, {showTitle}, {year}.");
    }
}