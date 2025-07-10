using Trackster.Api.Features.Media.Importers.TraktImporter;
using Trackster.Api.Features.Media.Types;

namespace Trackster.Api.Features.Media;

public class MediaService
{
    private readonly IMediaRepository _mediaRepository;

    public MediaService(IMediaRepository mediaRepository)
    {
        _mediaRepository = mediaRepository;
    }

    public async Task<ImportMediaResponse> ImportMedia(ImportMediaRequest request)
    {
        if (request.Type == ImportType.Trakt && request.Username != null)
        {
            var provider = new TraktImportProvider();
            var movies = await provider.GetMovies(request.Username);
            var shows = await provider.GetShows(request.Username);

            _mediaRepository.ImportMovies(request.Username, movies);
            _mediaRepository.ImportShows(request.Username,shows);
        }
        
        return new ImportMediaResponse();
    }
}