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

        if (form.TryGetValue("payload", out var payloadJson))
        {
            var payloadString = payloadJson.ToString();
            
            Console.WriteLine(payloadString);

            var parsed = HttpUtility.ParseQueryString(payloadString);

            var webhookRequest = new PlexWebhookRequest
            {
                @event = parsed["event"],
                user = bool.TryParse(parsed["user"], out var user) && user,
                owner = bool.TryParse(parsed["owner"], out var owner) && owner,
                Account = JsonConvert.DeserializeObject<PlexWebhookRequest.PlexAccount>(parsed["Account"]),
                Server = JsonConvert.DeserializeObject<PlexWebhookRequest.PlexServer>(parsed["Server"]),
                Player = JsonConvert.DeserializeObject<PlexWebhookRequest.PlexPlayer>(parsed["Player"]),
                Metadata = JsonConvert.DeserializeObject<PlexWebhookRequest.PlexMetadata>(parsed["Metadata"])
            };

            _service.HandlePlexWebhook(webhookRequest);

            return Ok();
        }

        return BadRequest("No payload found.");
    }
}