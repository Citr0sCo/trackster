using Trackster.Api.Features.Webhook.Types;
using Newtonsoft.Json;

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
