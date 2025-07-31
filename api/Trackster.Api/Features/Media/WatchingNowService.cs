using Trackster.Api.Core.Events.Types;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Media.Types;
using Trackster.Api.Features.WebSockets.Types;

namespace Trackster.Api.Features.Media;

public class WatchingNowService : ISubscriber
{
    private bool _isStarted = false;
    private static WatchingNowService _instance;

    private readonly Dictionary<string, WatchingMovieRecord> _watchingNowMovies;
    private readonly Dictionary<string, WatchingEpisodeRecord> _watchingNowEpisodes;

    public WatchingNowService()
    {
        _watchingNowMovies = new Dictionary<string, WatchingMovieRecord>();
        _watchingNowEpisodes = new Dictionary<string, WatchingEpisodeRecord>();
    }
    
    public static WatchingNowService Instance()
    {
        if (_instance == null)
            _instance = new WatchingNowService();

        return _instance;
    }
    
    public void MarkAsWatchingMovie(string username, MovieRecord movie, int millisecondsWatched, int duration)
    {
        _watchingNowMovies[username] = new WatchingMovieRecord
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
                Data = _watchingNowMovies[username]
            }
        });
    }

    public void MarkAsWatchingEpisode(string username, EpisodeRecord episode, int millisecondsWatched, int duration)
    {
        _watchingNowEpisodes[username] = new WatchingEpisodeRecord
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
                Data = _watchingNowEpisodes[username]
            }
        });
    }

    public void MarkAsStoppedWatchingMovie(string username)
    {
        if (_watchingNowMovies.ContainsKey(username))
            _watchingNowMovies.Remove(username);
        
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

    public void MarkAsStoppedWatchingEpisode(string username)
    {
        if (_watchingNowEpisodes.ContainsKey(username))
            _watchingNowEpisodes.Remove(username);
        
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

    public void MarkAsPausedWatchingMovie(string username, MovieRecord movie, int millisecondsWatched, int duration)
    {
        _watchingNowMovies[username] = new WatchingMovieRecord
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
                Data = _watchingNowMovies[username]
            }
        });
    }

    public void MarkAsPausedWatchingEpisode(string username, EpisodeRecord episode, int millisecondsWatched, int duration)
    {
        _watchingNowEpisodes[username] = new WatchingEpisodeRecord
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
                Data = _watchingNowEpisodes[username]
            }
        });
    }

    public WatchingMovieRecord? GetCurrentlyWatchingMovie()
    {
        if (_watchingNowMovies.TryGetValue("citr0s", out var movie))
            return movie;

        return null;
    }

    public WatchingEpisodeRecord? GetCurrentlyWatchingEpisode()
    {
        if (_watchingNowEpisodes.TryGetValue("citr0s", out var episode))
            return episode;
        
        return null;
    }

    public void OnStarted()
    {
        _isStarted = true;

        Task.Run(() =>
        {
            while (_isStarted)
            {
                var currentlyWatchingMovie = GetCurrentlyWatchingMovie();
                
                if(currentlyWatchingMovie != null)
                {
                    var diff = DateTime.Now - currentlyWatchingMovie.LastUpdatedAt;
                    currentlyWatchingMovie.MillisecondsWatched += diff.Milliseconds;
                    MarkAsWatchingMovie("citr0s", currentlyWatchingMovie.Movie, currentlyWatchingMovie.MillisecondsWatched, currentlyWatchingMovie.Duration);
                    
                    WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.WatchingNowMovie, new
                    {
                        Response = new
                        {
                            Data = currentlyWatchingMovie
                        }
                    });
                }
                
                var currentlyWatchingEpisode = GetCurrentlyWatchingEpisode();
                
                if(currentlyWatchingEpisode != null)
                {
                    var diff = DateTime.Now - currentlyWatchingEpisode.LastUpdatedAt;
                    currentlyWatchingEpisode.MillisecondsWatched += diff.Milliseconds;
                    MarkAsWatchingEpisode("citr0s", currentlyWatchingEpisode.Episode, currentlyWatchingEpisode.MillisecondsWatched, currentlyWatchingEpisode.Duration);
                    
                    WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.WatchingNowEpisode, new
                    {
                        Response = new
                        {
                            Data = currentlyWatchingEpisode
                        }
                    });
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