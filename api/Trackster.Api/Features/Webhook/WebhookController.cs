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

             var keyValuePairs = payloadString.Split(' ', StringSplitOptions.RemoveEmptyEntries);

             var dict = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
             foreach (var pair in keyValuePairs)
             {
                 var separatorIndex = pair.IndexOf('=');
                 if (separatorIndex > 0 && separatorIndex < pair.Length - 1)
                 {
                     var key = pair.Substring(0, separatorIndex);
                     var value = pair.Substring(separatorIndex + 1);
                     dict[key] = Uri.UnescapeDataString(value);
                 }
             }

            var webhookRequest = new PlexWebhookRequest
            {
                @event = dict["event"],
                user = bool.TryParse(dict["user"], out var user) && user,
                owner = bool.TryParse(dict["owner"], out var owner) && owner,
                Account = string.IsNullOrWhiteSpace(dict["Account"]) ? null : JsonConvert.DeserializeObject<PlexWebhookRequest.PlexAccount>(dict["Account"]),
                Server = string.IsNullOrWhiteSpace(dict["Server"]) ? null : JsonConvert.DeserializeObject<PlexWebhookRequest.PlexServer>(dict["Server"]),
                Player = string.IsNullOrWhiteSpace(dict["Player"]) ? null : JsonConvert.DeserializeObject<PlexWebhookRequest.PlexPlayer>(dict["Player"]),
                Metadata = string.IsNullOrWhiteSpace(dict["Metadata"]) ? null : JsonConvert.DeserializeObject<PlexWebhookRequest.PlexMetadata>(dict["Metadata"])
            };

            _service.HandlePlexWebhook(webhookRequest);

            return Ok();
        }

        return BadRequest("No payload found.");
    }
}