using Trackster.Api.Features.Webhook.Types;

namespace Trackster.Api.Features.Webhook;

public class PlexWebhookService
{
    public void HandlePlexWebhook(PlexWebhookRequest? parsedJson)
    {
        Console.WriteLine("--- Plex Webhook Parse Start ---");
        Console.WriteLine(JsonConvert.SerializeObject(parsedJson));
        Console.WriteLine("--- Plex Webhook Parse End ---");
    }
}