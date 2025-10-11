using Newtonsoft.Json;
using Trackster.Api.Features.Media;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.Movies;
using Trackster.Api.Features.Shows;
using Trackster.Api.Features.Users;
using Trackster.Api.Features.Users.Types;
using Trackster.Api.Features.Webhooks.Types;

namespace Trackster.Api.Features.Webhooks;

public class PlexWebhookService
{
    private readonly MediaService _mediaService;

    public PlexWebhookService()
    {
        _mediaService = new MediaService(new MoviesService(new MoviesRepository()), new ShowsService(new ShowsRepository()), new UsersService(new UsersRepository()));
    }

    public async Task HandlePlexWebhook(PlexWebhookRequest? parsedJson, User user)
    {
        Console.WriteLine("--- Plex Webhook Parse Start ---");
        Console.WriteLine("Event - " + parsedJson.Event);
        Console.WriteLine("Account - " + JsonConvert.SerializeObject(parsedJson.Account, Formatting.Indented));
        //Console.WriteLine("Server - " + JsonConvert.SerializeObject(parsedJson.Server, Formatting.Indented));
        Console.WriteLine("Metadata - " + JsonConvert.SerializeObject(parsedJson.Metadata, Formatting.Indented));
        //Console.WriteLine("Player - " + JsonConvert.SerializeObject(parsedJson.Player, Formatting.Indented));
        Console.WriteLine("--- Plex Webhook Parse End ---");

        if (parsedJson.Account.Title.ToLower() != user.Username.ToLower())
            return;

        var mediaType = parsedJson.Metadata.Type.ToLower();
        var eventType = parsedJson.Event.ToLower();

        if (eventType == "media.scrobble")
        {
            if (parsedJson.Metadata.Year > 0)
            {
                await _mediaService.MarkMediaAsWatched(new MarkMediaAsWatchedRequest
                {
                    Username = user.Username,
                    MediaType = mediaType,
                    Year = parsedJson.Metadata.Year,
                    Title = parsedJson.Metadata.Title,
                    ParentTitle = parsedJson.Metadata.ParentTitle,
                    GrandParentTitle = parsedJson.Metadata.GrandparentTitle,
                    SeasonNumber = parsedJson.Metadata.ParentIndex,
                });
            }
        }

        if (eventType == "media.play" || eventType == "media.resume")
        {
            await _mediaService.MarkMediaAsWatchingNow(new MarkMediaAsWatchingNowRequest
            {
                UserIdentifier = user.Identifier,
                MediaType = mediaType,
                Year = parsedJson.Metadata.Year,
                Title = parsedJson.Metadata.Title,
                ParentTitle = parsedJson.Metadata.ParentTitle,
                GrandParentTitle = parsedJson.Metadata.GrandparentTitle,
                MillisecondsWatched = parsedJson.Metadata.ViewOffsetInMilliseconds,
                Duration = parsedJson.Metadata.Duration,
                SeasonNumber = parsedJson.Metadata.ParentIndex
            });
        }
        
        if (eventType == "media.pause")
        {
            await _mediaService.PauseMediaAsWatchingNow(new MarkMediaAsPausedRequest
            {
                UserIdentifier = user.Identifier, 
                MediaType = mediaType, 
                Year = parsedJson.Metadata.Year, 
                Title = parsedJson.Metadata.Title, 
                ParentTitle = parsedJson.Metadata.ParentTitle, 
                GrandParentTitle = parsedJson.Metadata.GrandparentTitle, 
                SeasonNumber = parsedJson.Metadata.ParentIndex, 
                MillisecondsWatched = parsedJson.Metadata.ViewOffsetInMilliseconds, 
                Duration = parsedJson.Metadata.Duration
            });
        }

        if (eventType == "media.stop")
            _mediaService.RemoveMediaAsWatchingNow(user.Identifier, mediaType);
    }
}