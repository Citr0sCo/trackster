using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Features.Media;
using Trackster.Api.Features.Movies;
using Trackster.Api.Features.Sessions;
using Trackster.Api.Features.Shows;
using Trackster.Api.Features.Users;

namespace Trackster.Api.Features.Posters;

[ApiController]
[Route("api/[controller]")]
public class PostersController : ControllerBase
{
    private readonly MediaService _service;

    public PostersController()
    {
        _service = new MediaService(new MoviesService(new MoviesRepository()), new ShowsService(new ShowsRepository()), new UsersService(new UsersRepository()), new SessionService(new SessionRepository()));
    }
    
    [HttpGet("")]
    public async Task<IActionResult> GetHistoryForUser()
    {
        var response = await _service.GetPosters();
        
        return Ok(response);
    }
}