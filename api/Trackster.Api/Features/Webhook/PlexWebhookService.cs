using Trackster.Api.Features.Webhook.Types;
using Newtonsoft.Json;

namespace Trackster.Api.Features.Webhook;

public class PlexWebhookService
{
    public void HandlePlexWebhook(PlexWebhookRequest? parsedJson)
    {
        Console.WriteLine("--- Plex Webhook Parse Start ---");
        Console.WriteLine(parsedJson.Event);
        Console.WriteLine(JsonConvert.SerializeObject(parsedJson.Account, Formatting.Indented));
        Console.WriteLine(JsonConvert.SerializeObject(parsedJson.Server, Formatting.Indented));
        Console.WriteLine(JsonConvert.SerializeObject(parsedJson.Metadata, Formatting.Indented));
        Console.WriteLine(JsonConvert.SerializeObject(parsedJson.Player, Formatting.Indented));
        Console.WriteLine("--- Plex Webhook Parse End ---");
    }
}
