using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Attributes;
using Trackster.Api.Features.Sessions;
using Trackster.Api.Features.Users;

namespace Trackster.Api.Features.Settings;

[AuthRequired]
[ApiController]
[Route("api/[controller]")]
public class SettingsController : ControllerBase
{
    private readonly SettingsService _service;
    private readonly SessionHelper _sessionHelper;

    public SettingsController()
    {
        _service = new SettingsService(new SettingsRepository());
        _sessionHelper = new SessionHelper(new SessionService(new SessionRepository()), new UsersService(new UsersRepository()));
    }
    
    [HttpGet("")]
    public IActionResult GetSettings()
    {
        return Ok(_service.GetSettings(_sessionHelper.GetSessionId(HttpContext)));
    }
}