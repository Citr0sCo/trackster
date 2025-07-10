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
    public async Task<ImportMediaResponse> ImportLinks([FromBody]ImportMediaRequest request)
    {
        return await _service.ImportMedia(request);
    }
}