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

    public List<Movie> GetAllMovies(string username)
    {
        var movies = _mediaRepository.GetAllMovies(username);

        var mappedMovies = movies.ConvertAll(x => new Movie
        {
            Identifier = x.Identifier,
            Title = x.Title,
            Year = x.Year,
            TMDB =  x.TMDB,
            Poster = x.Poster,
            Overview = x.Overview
        });
        
        return mappedMovies;
    }
}