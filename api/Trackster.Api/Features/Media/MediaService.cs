using Trackster.Api.Features.Media.Types;

namespace Trackster.Api.Features.Media;

public class MediaService
{
    private readonly ILinksRepository _linksRepository;

    public MediaService(ILinksRepository linksRepository)
    {
        _linksRepository = linksRepository;
    }

    public ImportMediaResponse ImportMedia(ImportMediaRequest request)
    {
        throw new NotImplementedException();
    }
}