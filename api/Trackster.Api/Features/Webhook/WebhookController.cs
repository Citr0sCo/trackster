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

            var parsed = HttpUtility.ParseQueryString(payloadString);

            var eventName = parsed["event"];
            var user = parsed["user"];
            var owner = parsed["owner"];
            var accountJson = parsed["Account"];
            var serverJson = parsed["Server"];
            var playerJson = parsed["Player"];
            var metadataJson = parsed["Metadata"];

            var account = JsonConvert.DeserializeObject<PlexWebhookRequest.PlexAccount>(accountJson);
            var server = JsonConvert.DeserializeObject<PlexWebhookRequest.PlexServer>(serverJson);
            var player = JsonConvert.DeserializeObject<PlexWebhookRequest.PlexPlayer>(playerJson);
            var metadata = JsonConvert.DeserializeObject<PlexWebhookRequest.PlexMetadata>(metadataJson);
        
            _service.HandlePlexWebhook(eventName, user, owner, account, server, player, metadata);
            
            return Ok();
        }

        return BadRequest("No payload found.");
    }
}