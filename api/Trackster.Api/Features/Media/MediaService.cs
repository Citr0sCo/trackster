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
        Console.WriteLine($"Marking {showTitle} episode {episodeTitle} as watched by {year} season number {seasonNumber}.");
        
        var searchResults = await _detailsProvider.FindShowByTitleAndYear(showTitle, year);
        var tmdbReference = searchResults.Results.FirstOrDefault()?.Id.ToString();
        var parsedShow = await _detailsProvider.GetDetailsForShow(tmdbReference ?? "");
        var parsedSeason = await _detailsProvider.GetDetailsForSeason(parsedShow.Identifier, seasonNumber);
        var parsedEpisode = parsedSeason.Episodes.FirstOrDefault(x => x.Name.ToLower() == episodeTitle.ToLower());

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
            Number = seasonNumber
        };

        var episode = new EpisodeRecord
        {
            Identifier = Guid.NewGuid(),
            Season = season,
            Number = parsedEpisode.EpisodeNumber
        };
        
        _mediaRepository.ImportEpisode("citr0s", show, season, episode);
        
        Console.WriteLine($"Marked {showTitle} episode {episodeTitle} as watched by {year} season number {seasonNumber}.");
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