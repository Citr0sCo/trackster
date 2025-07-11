using Microsoft.AspNetCore.Mvc;
using Trackster.Api.Features.Webhook.Types;

namespace Trackster.Api.Features.Webhook;

[ApiController]
[Route("api/[controller]")]
public class WebhookController : ControllerBase
{
    private readonly PlexWebhookService _service;

    public WebhookController()
    {
        _service = new PlexWebhookService();
    }
    
    [HttpPost("plex/{apiKey}")]
    public IActionResult HandlePlexWebhook([FromBody]PlexWebhookRequest request)
    {
        _service.HandlePlexWebhook(request);
        return Ok();
    }
}