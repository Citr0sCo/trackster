using Trackster.Api.Features.Webhook.Types;
using Newtonsoft.Json;
using Trackster.Api.Features.Media;

namespace Trackster.Api.Features.Webhook;

public class PlexWebhookService
{
    private readonly MediaService _mediaService;

    public PlexWebhookService()
    {
        _mediaService = new MediaService(new MediaRepository());
    }
    
    public void HandlePlexWebhook(PlexWebhookRequest? parsedJson)
    {
        Console.WriteLine("--- Plex Webhook Parse Start ---");
        Console.WriteLine("Event - " + parsedJson.Event);
        Console.WriteLine("Account - " + JsonConvert.SerializeObject(parsedJson.Account, Formatting.Indented));
        Console.WriteLine("Server - " + JsonConvert.SerializeObject(parsedJson.Server, Formatting.Indented));
        Console.WriteLine("Metadata - " + JsonConvert.SerializeObject(parsedJson.Metadata, Formatting.Indented));
        Console.WriteLine("Player - " + JsonConvert.SerializeObject(parsedJson.Player, Formatting.Indented));
        Console.WriteLine("--- Plex Webhook Parse End ---");
        
        if (parsedJson.Event.ToLower() == "media.scrobble" && parsedJson.Metadata.Type.ToLower() == "movie")
            _mediaService.MarkMovieAsWatched(parsedJson.Metadata.Title, parsedJson.Metadata.Year);
        
        if (parsedJson.Event.ToLower() == "media.scrobble" && parsedJson.Metadata.Type.ToLower() == "episode")
            _mediaService.MarkEpisodeAsWatched(parsedJson.Metadata.Title, parsedJson.Metadata.ParentTitle, parsedJson.Metadata.GrandparentTitle, parsedJson.Metadata.Year);

        if (parsedJson.Event.ToLower() == "media.play")
            _mediaService.MarkMediaAsWatchingNow(parsedJson.Metadata.Title, parsedJson.Metadata.ParentTitle, parsedJson.Metadata.GrandparentTitle, parsedJson.Metadata.Year);
        
        if (parsedJson.Event.ToLower() == "media.stop")
            _mediaService.RemoveMediaAsWatchingNow(parsedJson.Metadata.Title, parsedJson.Metadata.ParentTitle, parsedJson.Metadata.GrandparentTitle, parsedJson.Metadata.Year);
    }
}
