using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Features.Authentication.Types;

namespace Trackster.Api.Features.Authentication;

[ApiController]
[Route("api/[controller]")]
public class AuthenticationController : ControllerBase
{
    private readonly AuthenticationService _service;

    public AuthenticationController()
    {
        _service = new AuthenticationService(new AuthenticationRepository());
    }
    
    [HttpPost("sign-in")]
    public async Task<SignInResponse> SignIn([FromBody]SignInRequest request)
    {
        return await _service.SignIn(request);
    }
}