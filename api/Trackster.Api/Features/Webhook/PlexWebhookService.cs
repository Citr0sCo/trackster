using Trackster.Api.Features.Webhook.Types;

namespace Trackster.Api.Features.Webhook;

public class PlexWebhookService
{
    public void HandlePlexWebhook(string? eventName, string? user, string? owner, PlexWebhookRequest.PlexAccount? account, PlexWebhookRequest.PlexServer? server, PlexWebhookRequest.PlexPlayer? player, PlexWebhookRequest.PlexMetadata? metadata)
    {
        Console.WriteLine("--- Plex Webhook Parse Start ---");
        Console.WriteLine(eventName);
        Console.WriteLine(user);
        Console.WriteLine(owner);
        Console.WriteLine(account);
        Console.WriteLine(server);
        Console.WriteLine(player);
        Console.WriteLine(metadata);
        Console.WriteLine("--- Plex Webhook Parse End ---");
    }
}