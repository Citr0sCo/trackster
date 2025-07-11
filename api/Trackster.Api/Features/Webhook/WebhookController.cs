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
    public async Task<IActionResult> HandlePlexWebhook()
    {
        // Parse the form data
        var form = await Request.ReadFormAsync();

        // Plex sends the JSON in a form field called "payload"
        if (form.TryGetValue("payload", out var payloadJson))
        {
            // Do something with the event
            Console.WriteLine($"Received Plex payload: {payloadJson}");

            return Ok();
        }

        return BadRequest("No payload found.");
    }
}