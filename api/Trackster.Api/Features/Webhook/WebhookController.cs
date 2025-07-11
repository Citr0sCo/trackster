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

        if (form.TryGetValue("payload", out var payloadJson))
        {
            Console.WriteLine("--- Plex Event Start ---");
            Console.WriteLine(payloadJson);
            Console.WriteLine("--- Plex Event End ---");
            _service.HandlePlexWebhook(JsonConvert.DeserializeObject<PlexWebhookRequest>(payloadJson));
            return Ok();
        }

        return BadRequest("No payload found.");
    }
}