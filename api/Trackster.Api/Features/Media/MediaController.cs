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
    
    [HttpGet("movies")]
    public List<Movie> GetAllMovies([FromQuery]string username)
    {
        return _service.GetAllMovies(username);
    }
    
    [HttpPost("import")]
    public async Task<ImportMediaResponse> ImportLinks([FromBody]ImportMediaRequest request)
    {
        return await _service.ImportMedia(request);
    }
}