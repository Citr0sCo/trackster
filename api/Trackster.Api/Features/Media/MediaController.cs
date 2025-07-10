using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Features.Media.Types;

namespace Trackster.Api.Features.Media;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly MediaService _service;

    public MediaController()
    {
        _service = new MediaService(new MediaRepository());
    }
    
    [HttpPost("import")]
    public ImportMediaResponse ImportLinks([FromBody]ImportMediaRequest request)
    {
        return _service.ImportMedia(request);
    }
}