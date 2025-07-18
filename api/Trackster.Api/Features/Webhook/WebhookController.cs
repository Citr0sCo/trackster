using System.Web;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
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
        var form = await Request.ReadFormAsync();
        
        if (!form.TryGetValue("payload", out var payloadJson))
            return BadRequest("No payload found.");

        try
        {
            var webhookRequest = JsonConvert.DeserializeObject<PlexWebhookRequest>(payloadJson);
            
            if (webhookRequest == null)
                return BadRequest("Invalid payload");

            _service.HandlePlexWebhook(webhookRequest);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[FATAL] - Exception thrown in Plex Webhook - {e.Message}");
            Console.WriteLine(e);
            return StatusCode(500, "Internal server error");
        }
    }
}