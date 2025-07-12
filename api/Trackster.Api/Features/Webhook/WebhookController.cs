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
        
        Console.WriteLine($"Raw request: {form}");

        if (!form.TryGetValue("payload", out var payloadJson))
            return BadRequest("No payload found.");

        Console.WriteLine($"Parsed payload: {payloadJson}");
        
        try
        {
            var webhookRequest = JsonConvert.DeserializeObject<PlexWebhookRequest>(payloadJson);
            
            if (webhookRequest == null)
                return BadRequest("Invalid payload");

            _service.HandlePlexWebhook(webhookRequest);

            return Ok();
        }
        catch (Exception)
        {
            return StatusCode(500, "Internal server error");
        }
    }
}