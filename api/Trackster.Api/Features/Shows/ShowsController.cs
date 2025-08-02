using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Features.Shows.Types;

namespace Trackster.Api.Features.Shows;

[ApiController]
[Route("api/[controller]")]
public class ShowsController : ControllerBase
{
    private readonly ShowsService _service;

    public ShowsController()
    {
        _service = new ShowsService(new ShowsRepository());
    }
    
    [HttpGet("")]
    public GetAllShowsResponse GetAllShows([FromQuery]string username, [FromQuery]int results = 50, [FromQuery]int page = 1)
    {
        return _service.GetAllWatchedShows(username, results, page);
    }
    
    [HttpGet("{slug}")]
    public IActionResult GetShowBySlug([FromRoute]string slug)
    {
        var response = _service.GetShowBySlug(slug);
        
        if(response == null)
            return NotFound();
        
        return Ok(response);
    }
    
    [HttpGet("{slug}/seasons/{seasonNumber}")]
    public IActionResult GetSeasonByNumber([FromRoute]string slug, [FromRoute]int seasonNumber)
    {
        var response = _service.GetSeasonByNumber(slug, seasonNumber);
        
        if(response == null)
            return NotFound();
        
        return Ok(response);
    }
    
    [HttpGet("{slug}/seasons/{seasonNumber}/episodes/{episodeNumber}")]
    public IActionResult GetSeasonByNumber([FromRoute]string slug, [FromRoute]int seasonNumber,  [FromRoute]int episodeNumber)
    {
        var response = _service.GetEpisodeByNumber(slug, seasonNumber, episodeNumber);
        
        if(response == null)
            return NotFound();
        
        return Ok(response);
    }
    
    [HttpGet("{slug}/seasons/{seasonNumber}/episodes/{episodeNumber}/history")]
    public IActionResult GetWatchedHistoryByEpisodeNumber([FromQuery]string username, [FromRoute]string slug, [FromRoute]int seasonNumber,  [FromRoute]int episodeNumber)
    {   
        var response = _service.GetWatchedHistoryByEpisodeNumber(username, slug, seasonNumber, episodeNumber);
        
        if(response == null)
            return NotFound();
        
        return Ok(response);
    }
}