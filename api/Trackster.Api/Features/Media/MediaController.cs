using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Attributes;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.Movies;
using Trackster.Api.Features.Sessions;
using Trackster.Api.Features.Shows;
using Trackster.Api.Features.Users;

namespace Trackster.Api.Features.Media;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly MediaService _service;
    private readonly SessionHelper _sessionHelper;

    public MediaController()
    {
        var usersService = new UsersService(new UsersRepository());
        
        _service = new MediaService(new MoviesService(new MoviesRepository()), new ShowsService(new ShowsRepository()), usersService, new SessionService(new SessionRepository()));
        _sessionHelper = new SessionHelper(new SessionService(new SessionRepository()), usersService);
    }
    
    [AuthRequired]
    [HttpGet("history")]
    public GetHistoryForUserResponse GetHistoryForUser([FromQuery]string username, [FromQuery]int results = 50, [FromQuery]int page = 1)
    {
        return _service.GetHistoryForUser(username, results, page);
    }
    
    [AuthRequired]
    [HttpGet("stats")]
    public GetStatsResponse GetStats([FromQuery]string username)
    {
        return _service.GetStats(username);
    }
        
    [AuthRequired]
    [HttpGet("stats/calendar")]
    public GetStatsForCalendarResonse GetStatsForCalendar([FromQuery]string username, [FromQuery]int daysInThePast = 365)
    {
        return _service.GetStatsForCalendar(username, daysInThePast);
    }
    
    [HttpGet("import")]
    public async Task<IActionResult> ImportMedia([FromQuery] Guid token, [FromQuery] bool isDebug = false)
    {
        Response.ContentType = "text/event-stream";

        var user = await _sessionHelper.GetUser(token);

        if (user == null)
            return BadRequest();
        
        await foreach (var item in _service.ImportMedia(new ImportMediaRequest { Type = ImportType.Trakt, Username = user.Username, Debug = isDebug }))
        {
            var json = System.Text.Json.JsonSerializer.Serialize(item);
            await Response.WriteAsync($"data: {json}\n\n");
            await Response.Body.FlushAsync();
        }

        return Ok();
    }
}