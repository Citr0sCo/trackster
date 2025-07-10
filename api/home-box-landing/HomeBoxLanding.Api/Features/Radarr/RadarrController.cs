using HomeBoxLanding.Api.Features.Links;
using HomeBoxLanding.Api.Features.Radarr.Types;
using Microsoft.AspNetCore.Mvc;

namespace HomeBoxLanding.Api.Features.Radarr;

[ApiController]
[Route("api/[controller]")]
public class RadarrController : ControllerBase
{
    private readonly RadarrService _service;

    public RadarrController()
    {
        _service = new RadarrService(new LinksService(new LinksRepository()));
    }

    [HttpGet("activity")]
    public RadarrActivityResponse GetActivity()
    {
        return _service.GetActivity();
    }
}