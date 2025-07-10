using HomeBoxLanding.Api.Features.Links;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HomeBoxLanding.Api.Features.Files;

[ApiController]
[Route("api/[controller]")]
public class FilesController : ControllerBase
{
    private readonly IMemoryCache _memoryCache;
    private readonly LinksService _linkService;
    private readonly FilesCache _filesCache;

    public FilesController(IMemoryCache memoryCache)
    {
        _memoryCache = memoryCache;
        _linkService = new LinksService(new LinksRepository());
        _filesCache = FilesCache.Instance();
    }
    
    [HttpGet("{linkReference:guid}")]
    public IActionResult GetFile([FromRoute] Guid linkReference)
    {
        Response.Headers["Cache-Control"] = "public, max-age=3600";
        
        if (_filesCache.Has(linkReference) && _memoryCache.TryGetValue(linkReference, out byte[] fileData))
        {
            var linkUrl = _filesCache.Get(linkReference);
            return File(fileData, "application/octet-stream", Path.GetFileName(linkUrl));
        }

        var link = _linkService.GetLinkByReference(linkReference);

        if (link == null)
            return NotFound($"Link not found by reference {linkReference}.");
        
        if (System.IO.File.Exists(link.IconUrl) == false)
            return NotFound($"File not found by url {link.IconUrl}.");
        
        var fileStream = new FileStream(link.IconUrl, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, useAsync: true);
            
        _filesCache.Remove(linkReference);
        _filesCache.Add(linkReference, link.IconUrl);
        _memoryCache.Set(linkReference, fileStream, TimeSpan.FromMinutes(10));
        
        return File(fileStream, "application/octet-stream", Path.GetFileName(link.IconUrl));
    }

    [HttpDelete("cache")]
    public IActionResult BustCache()
    {
        _filesCache.BustCache();
        return Ok();
    }
}