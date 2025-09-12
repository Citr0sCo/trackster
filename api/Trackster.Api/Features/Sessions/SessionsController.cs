using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Attributes;
using Trackster.Api.Features.Users;

namespace Trackster.Api.Features.Sessions;

[AuthRequired]
[ApiController]
[Route("api/[controller]")]
public class SessionsController : ControllerBase
{
    private readonly SessionService _service;
    private readonly SessionHelper _sessionHelper;
    private readonly UsersService _userService;

    public SessionsController()
    {
        _service = new SessionService(new SessionRepository());
        _userService = new UsersService(new UsersRepository());
        _sessionHelper = new SessionHelper(_service, _userService);
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetUser()
    {
        var sessionId = _sessionHelper.GetSessionId(HttpContext);

        var session = await _service.GetSession(sessionId);

        if (session == null)
            return NotFound();

        var user = await _userService.GetUserByReference(sessionId, session.UserIdentifier());
        
        return Ok(user);
    }
}