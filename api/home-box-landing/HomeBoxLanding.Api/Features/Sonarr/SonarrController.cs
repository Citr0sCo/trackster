using HomeBoxLanding.Api.Features.Links;
using HomeBoxLanding.Api.Features.Sonarr.Types;
using Microsoft.AspNetCore.Mvc;

namespace HomeBoxLanding.Api.Features.Sonarr;

[ApiController]
[Route("api/[controller]")]
public class SonarrController : ControllerBase
{
    private readonly SonarrService _service;

    public SonarrController()
    {
        _service = new SonarrService(new LinksService(new LinksRepository()));
    }

    [HttpGet("activity")]
    public SonarrActivityResponse GetActivity()
    {
        return _service.GetActivity();
    }
}