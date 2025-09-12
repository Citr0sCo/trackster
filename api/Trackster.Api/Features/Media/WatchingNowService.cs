using Trackster.Api.Core.Events.Types;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.WebSockets.Types;

namespace Trackster.Api.Features.Media;

public class WatchingNowService : ISubscriber
{
    private bool _isStarted = false;
    private static WatchingNowService? _instance;

    private readonly Dictionary<Guid, WatchingMovieRecord> _watchingNowMovies;
    private readonly Dictionary<Guid, WatchingEpisodeRecord> _watchingNowEpisodes;

    public WatchingNowService()
    {
        _watchingNowMovies = new Dictionary<Guid, WatchingMovieRecord>();
        _watchingNowEpisodes = new Dictionary<Guid, WatchingEpisodeRecord>();
    }
    
    public static WatchingNowService Instance()
    {
        if (_instance == null)
            _instance = new WatchingNowService();

        return _instance;
    }
    
    public void MarkAsWatchingMovie(Guid userReference, MovieRecord movie, int millisecondsWatched, int duration)
    {
        _watchingNowMovies[userReference] = new WatchingMovieRecord
        {
            Action = WatchingAction.Start.ToString(),
            Movie = movie,
            MillisecondsWatched = millisecondsWatched,
            Duration = duration,
            LastUpdatedAt = DateTime.Now
        };
        
        WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.WatchingNowMovie, new
        {
            Response = new
            {
                Data = _watchingNowMovies[userReference]
            }
        });
    }

    public void MarkAsWatchingEpisode(Guid userReference, EpisodeRecord episode, int millisecondsWatched, int duration)
    {
        _watchingNowEpisodes[userReference] = new WatchingEpisodeRecord
        {
            Action = WatchingAction.Start.ToString(),
            Episode = episode,
            MillisecondsWatched = millisecondsWatched,
            Duration = duration,
            LastUpdatedAt = DateTime.Now
        };
        
        WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.WatchingNowEpisode, new
        {
            Response = new
            {
                Data = _watchingNowEpisodes[userReference]
            }
        });
    }

    public void MarkAsStoppedWatchingMovie(Guid userReference)
    {
        if (_watchingNowMovies.ContainsKey(userReference))
            _watchingNowMovies.Remove(userReference);
        
        WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.WatchingNowMovie, new
        {
            Response = new
            {
                Data = new
                {
                    Action = WatchingAction.Stop.ToString()
                }
            }
        });
    }

    public void MarkAsStoppedWatchingEpisode(Guid userReference)
    {
        if (_watchingNowEpisodes.ContainsKey(userReference))
            _watchingNowEpisodes.Remove(userReference);
        
        WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.WatchingNowEpisode, new
        {
            Response = new
            {
                Data = new
                {
                    Action = WatchingAction.Stop.ToString()
                }
            }
        });
    }

    public void MarkAsPausedWatchingMovie(Guid userReference, MovieRecord movie, int millisecondsWatched, int duration)
    {
        _watchingNowMovies[userReference] = new WatchingMovieRecord
        {
            Action = WatchingAction.Paused.ToString(),
            Movie = movie,
            MillisecondsWatched = millisecondsWatched,
            Duration = duration,
            LastUpdatedAt = DateTime.Now
        };
        
        WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.WatchingNowMovie, new
        {
            Response = new
            {
                Data = _watchingNowMovies[userReference]
            }
        });
    }

    public void MarkAsPausedWatchingEpisode(Guid userReference, EpisodeRecord episode, int millisecondsWatched, int duration)
    {
        _watchingNowEpisodes[userReference] = new WatchingEpisodeRecord
        {
            Action = WatchingAction.Paused.ToString(),
            Episode = episode,
            MillisecondsWatched = millisecondsWatched,
            Duration = duration,
            LastUpdatedAt = DateTime.Now
        };
        
        WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.WatchingNowEpisode, new
        {
            Response = new
            {
                Data = _watchingNowEpisodes[userReference]
            }
        });
    }

    public Dictionary<Guid, WatchingMovieRecord> GetCurrentlyWatchingMovie()
    {
        return _watchingNowMovies;
    }

    public Dictionary<Guid, WatchingEpisodeRecord> GetCurrentlyWatchingEpisode()
    {
        return _watchingNowEpisodes;
    }

    public void OnStarted()
    {
        _isStarted = true;

        Task.Run(() =>
        {
            while (_isStarted)
            {
                var webSocketManager = WebSockets.WebSocketManager.Instance();
                
                var currentlyWatchingMovies = GetCurrentlyWatchingMovie();

                foreach (var user in currentlyWatchingMovies.Keys)
                {
                    currentlyWatchingMovies[user].MillisecondsWatched += 5000;
                    MarkAsWatchingMovie(user, currentlyWatchingMovies[user].Movie, currentlyWatchingMovies[user].MillisecondsWatched, currentlyWatchingMovies[user].Duration);

                    var webSocketSessionId = webSocketManager.GetWebsocketSessionIdByUserReference(user);

                    if (webSocketSessionId.HasValue)
                    {
                        webSocketManager.Send(webSocketSessionId.Value, WebSocketKey.WatchingNowMovie, new
                        {
                            Response = new
                            {
                                Data = currentlyWatchingMovies
                            }
                        });
                    }   
                }
                
                var currentlyWatchingEpisodes = GetCurrentlyWatchingEpisode();
                
                foreach (var user in currentlyWatchingEpisodes.Keys)
                {
                    currentlyWatchingEpisodes[user].MillisecondsWatched += 5000;
                    MarkAsWatchingEpisode(user, currentlyWatchingEpisodes[user].Episode, currentlyWatchingEpisodes[user].MillisecondsWatched, currentlyWatchingEpisodes[user].Duration);
                    
                    var webSocketSessionId = webSocketManager.GetWebsocketSessionIdByUserReference(user);

                    if (webSocketSessionId.HasValue)
                    {
                        webSocketManager.Send(webSocketSessionId.Value, WebSocketKey.WatchingNowEpisode, new
                        {
                            Response = new
                            {
                                Data = currentlyWatchingEpisodes
                            }
                        });
                    }
                }
                
                Thread.Sleep(5000);
            }
        }, CancellationToken.None);
    }

    public void OnStopping()
    {
        _isStarted = false;
    }

    public void OnStopped()
    {
        // Do nothing
    }
}