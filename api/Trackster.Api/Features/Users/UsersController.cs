using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Attributes;
using Trackster.Api.Features.Sessions;

namespace Trackster.Api.Features.Users;

[AuthRequired]
[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly UsersService _service;
    private readonly SessionHelper _sessionHelper;

    public UsersController()
    {
        _service = new UsersService(new UsersRepository());
        _sessionHelper = new SessionHelper();
    }
    
    [HttpGet("{reference}")]
    public async Task<IActionResult> GetUser(Guid reference)
    {
        return Ok(await _service.GetUserByReference(_sessionHelper.GetSessionId(HttpContext), reference));
    }
}