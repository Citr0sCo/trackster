using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Trackster.Api.Features.Users;
using Trackster.Api.Features.Webhooks.Types;

namespace Trackster.Api.Features.Webhooks;

[ApiController]
[Route("api/[controller]")]
public class WebhooksController : ControllerBase
{
    private readonly PlexWebhookService _service;
    private readonly WebhooksService _webhooksService;
    private readonly UsersService _userService;

    public WebhooksController()
    {
        _service = new PlexWebhookService();
        _webhooksService = new WebhooksService(new WebhooksRepository());
        _userService = new UsersService(new UsersRepository());
    }

    [HttpPost("plex/{apiKey}")]
    public async Task<IActionResult> HandlePlexWebhook(string apiKey)
    {
        var form = await Request.ReadFormAsync();

        if (!form.TryGetValue("payload", out var payloadJson))
            return BadRequest("No payload found.");

        try
        {
            var webhookRequest = JsonConvert.DeserializeObject<PlexWebhookRequest>(payloadJson);

            if (webhookRequest == null)
                return BadRequest("Invalid payload");

            var webhookResponse = await _webhooksService.GetWebhookByApiKey(apiKey);

            if (webhookResponse == null)
                return BadRequest("ApiKey not configured");

            var userResponse = await _userService.GetUserByReference(webhookResponse.UserIdentifier);

            await _service.HandlePlexWebhook(webhookRequest, userResponse.User.Username);

            return Ok();
        }
        catch (Exception e)
        {
            Console.WriteLine($"[FATAL] - Exception thrown in Plex Webhook - {e.Message}");
            Console.WriteLine(e);
            return StatusCode(500, "Internal server error");
        }
    }

    [HttpGet("{userReference}")]
    public async Task<IActionResult> GetWebhook(Guid userReference)
    {
        var user = await _userService.GetUserByReference(userReference);
        
        if (user.HasError)
            return BadRequest(user.Error?.UserMessage);
        
        var webhook = await _webhooksService.GetWebhookForUser(userReference);
        
        if (webhook == null)
            return NotFound("Webhook not found");
        
        return Ok(webhook);
    }

    [HttpPost("{userReference}")]
    public async Task<IActionResult> CreateWebhook(CreateWebhookRequest request)
    {
        var user = await _userService.GetUserByReference(request.UserIdentifier);
        
        if (user.HasError)
            return BadRequest(user.Error?.UserMessage);
        
        var existingWebhook = await _webhooksService.GetWebhookForUser(request.UserIdentifier);

        if (existingWebhook != null)
            return Ok(existingWebhook);
        
        var webhook = await _webhooksService.CreateWebhook(request.UserIdentifier, request.Provider);
        
        return Ok(webhook);
    }
}