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
        var searchResults = await _detailsProvider.FindMovieByTitleAndYear(title, year);
        var tmdbReference = searchResults.Results.FirstOrDefault()?.Id.ToString();
        var movie = await _detailsProvider.GetDetailsForMovie(tmdbReference ?? "");
        
        _mediaRepository.ImportMovie("citr0s", new Movie
        {
            Identifier = Guid.NewGuid(),
            Title = title,
            TMDB = tmdbReference,
            Year = year,
            Overview = movie.Overview,
            Poster = movie.PosterUrl,
            WatchedAt = DateTime.Now
        });
    }

    public void MarkEpisodeAsWatched(string episodeTitle, string seasonTitle, string showTitle, int year)
    {
        throw new NotImplementedException();
    }

    public void MarkMediaAsWatchingNow(string episodeTitle, string seasonTitle, string showTitle, int year)
    {
        throw new NotImplementedException();
    }

    public void RemoveMediaAsWatchingNow(string episodeTitle, string seasonTitle, string showTitle, int year)
    {
        throw new NotImplementedException();
    }
}