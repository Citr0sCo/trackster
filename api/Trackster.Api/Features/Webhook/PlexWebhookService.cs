using Newtonsoft.Json;
using Trackster.Api.Features.Webhook.Types;

namespace Trackster.Api.Features.Webhook;

public class PlexWebhookService
{
    public void HandlePlexWebhook(PlexWebhookRequest request)
    {
        Console.WriteLine("[INFO] --- Received Plex Event ---");
        Console.WriteLine(JsonConvert.SerializeObject(request));
    }
}