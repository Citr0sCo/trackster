using HomeBoxLanding.Api.Features.Spotify.Types;
using Microsoft.AspNetCore.Mvc;

namespace HomeBoxLanding.Api.Features.Spotify;

[ApiController]
[Route("api/[controller]")]
public class SpotifyController : ControllerBase
{
    private readonly SpotifyService _service;

    public SpotifyController()
    {
        _service = new SpotifyService();
    }

    [HttpPost("test")]
    public SpotifyImportSongsResponse GetActivity([FromBody] SpotifyTestRequest request)
    {
        return _service.GetActivity(request);
    }
}