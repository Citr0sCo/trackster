using Microsoft.AspNetCore.Mvc;

namespace HomeBoxLanding.Api.Features.Columns;

[ApiExplorerSettings(IgnoreApi = true)]
[Route("api/[controller]")]
public class ColumnsController : Controller
{
    [HttpGet("")]
    //[Administator]
    //[Authentication]
    public ActionResult Get()
    {
        return Ok();
    }
        
    [HttpPost("")]
    public ActionResult Create()
    {
        return Ok("A-OK");
    }
}