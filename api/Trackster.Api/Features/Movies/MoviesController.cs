using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Attributes;
using Trackster.Api.Features.Movies.Types;

namespace Trackster.Api.Features.Movies;

[AuthRequired]
[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly MoviesService _service;

    public MoviesController()
    {
        _service = new MoviesService(new MoviesRepository());
    }
    
    [HttpGet("")]
    public GetAllMoviesResponse GetAll([FromQuery]string username, [FromQuery]int results = 50, [FromQuery]int page = 1)
    {
        return _service.GetAllWatchedMovies(username, results, page);
    }
    
    [HttpGet("{slug}")]
    public IActionResult GetByIdentifier([FromRoute]string slug)
    {
        var response = _service.GetMovieBySlug(slug);
        
        if(response == null)
            return NotFound();
        
        return Ok(response);
    }
    
    [HttpPatch("{slug}")]
    public async Task<IActionResult> UpdateMetadata([FromRoute]string slug)
    {
        var response = await _service.ImportDataForMovie(slug);
        
        if(response == null)
            return NotFound();
        
        return Ok(response);
    }
    
    [HttpGet("{slug}/history")]
    public IActionResult GetWatchHistory([FromQuery]string username, [FromRoute]string slug)
    {   
        var response = _service.GetWatchedHistoryBySlug(username, slug);
        
        if(response == null)
            return NotFound();
        
        return Ok(response);
    }
}