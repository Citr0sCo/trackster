using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.Movies;
using Trackster.Api.Features.Shows;
using Trackster.Api.Features.Users;

namespace Trackster.Api.Features.Media;

[ApiController]
[Route("api/[controller]")]
public class MediaController : ControllerBase
{
    private readonly MediaService _service;

    public MediaController()
    {
        _service = new MediaService(new MoviesService(new MoviesRepository()), new ShowsService(new ShowsRepository()), new UsersService(new UsersRepository()));
    }
    
    [HttpGet("history")]
    public GetHistoryForUserResponse GetHistoryForUser([FromQuery]string username, [FromQuery]int results = 50, [FromQuery]int page = 1)
    {
        return _service.GetHistoryForUser(username, results, page);
    }
    
    [HttpGet("stats")]
    public GetStatsResponse GetStats([FromQuery]string username)
    {
        return _service.GetStats(username);
    }
    
    [HttpGet("stats/calendar")]
    public GetStatsForCalendarResonse GetStatsForCalendar([FromQuery]string username, [FromQuery]int daysInThePast = 365)
    {
        return _service.GetStatsForCalendar(username, daysInThePast);
    }
    
    [HttpPost("import")]
    public async Task<ImportMediaResponse> ImportMedia([FromBody]ImportMediaRequest request)
    {
        await Task.Run(async () =>
        {
            await _service.ImportMedia(request);
        });
        
        return new ImportMediaResponse();
    }
}