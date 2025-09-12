using Trackster.Api.Features.Webhook.Types;
using Newtonsoft.Json;
using Trackster.Api.Features.Media;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.Movies;
using Trackster.Api.Features.Shows;
using Trackster.Api.Features.Users;

namespace Trackster.Api.Features.Webhook;

public class PlexWebhookService
{
    private readonly MediaService _mediaService;

    public PlexWebhookService()
    {
        _mediaService = new MediaService(new MoviesService(new MoviesRepository()), new ShowsService(new ShowsRepository()), new UsersService(new UsersRepository()));
    }
    
    public async Task HandlePlexWebhook(PlexWebhookRequest? parsedJson)
    {
        Console.WriteLine("--- Plex Webhook Parse Start ---");
        Console.WriteLine("Event - " + parsedJson.Event);
        Console.WriteLine("Account - " + JsonConvert.SerializeObject(parsedJson.Account, Formatting.Indented));
        //Console.WriteLine("Server - " + JsonConvert.SerializeObject(parsedJson.Server, Formatting.Indented));
        Console.WriteLine("Metadata - " + JsonConvert.SerializeObject(parsedJson.Metadata, Formatting.Indented));
        //Console.WriteLine("Player - " + JsonConvert.SerializeObject(parsedJson.Player, Formatting.Indented));
        Console.WriteLine("--- Plex Webhook Parse End ---");

        var username = "citr0s";
        
        if (parsedJson.Account.Title.ToLower() != username.ToLower())
            return;

        var mediaType = parsedJson.Metadata.Type.ToLower();
        var eventType = parsedJson.Event.ToLower();
        
        if (eventType == "media.scrobble")
        {
            await _mediaService.MarkMediaAsWatched(new MarkMediaAsWatchedRequest
            {
                Username = username, 
                MediaType = mediaType, 
                Year = parsedJson.Metadata.Year, 
                Title = parsedJson.Metadata.Title, 
                ParentTitle = parsedJson.Metadata.ParentTitle, 
                GrandParentTitle = parsedJson.Metadata.GrandparentTitle, 
                ParentIndex = parsedJson.Metadata.ParentIndex
            });
        }

        if (eventType == "media.play" || eventType == "media.resume")
            _mediaService.MarkMediaAsWatchingNow(username, mediaType, parsedJson.Metadata.Year, parsedJson.Metadata.Title, parsedJson.Metadata.ParentTitle, parsedJson.Metadata.GrandparentTitle, parsedJson.Metadata.ParentIndex, parsedJson.Metadata.ViewOffsetInMilliseconds, parsedJson.Metadata.Duration);
        
        if (eventType == "media.stop" || eventType == "media.pause")
            _mediaService.RemoveMediaAsWatchingNow(username, mediaType, parsedJson.Metadata.Year, parsedJson.Metadata.Title, parsedJson.Metadata.ParentTitle, parsedJson.Metadata.GrandparentTitle, parsedJson.Metadata.ParentIndex);
        
        if (eventType == "media.pause")
            _mediaService.PauseMediaAsWatchingNow(username, mediaType, parsedJson.Metadata.Year, parsedJson.Metadata.Title, parsedJson.Metadata.ParentTitle, parsedJson.Metadata.GrandparentTitle, parsedJson.Metadata.ParentIndex, parsedJson.Metadata.ViewOffsetInMilliseconds, parsedJson.Metadata.Duration);
    }
}
