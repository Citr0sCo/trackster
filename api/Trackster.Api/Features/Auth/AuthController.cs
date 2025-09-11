using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Features.Auth.Types;

namespace Trackster.Api.Features.Auth;

[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly AuthenticationService _service;

    public AuthController()
    {
        _service = new AuthenticationService(new SessionService(new SessionRepository()));
    }
    
    [HttpPost("sign-in")]
    public async Task<IActionResult> SignIn([FromBody]SignInRequest request)
    {
        return Ok(await _service.SignIn(request));
    }
    
    [HttpDelete("sign-out/{reference}")]
    public async Task<IActionResult> SignIn(Guid reference)
    {
        return Ok(await _service.SignOut(reference));
    }
    
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody]RegisterRequest request)
    {
        return Ok(await _service.Register(request));
    }
}