using HomeBoxLanding.Api.Core.Events.Types;
using HomeBoxLanding.Api.Features.Links;
using HomeBoxLanding.Api.Features.Links.Types;
using HomeBoxLanding.Api.Features.Radarr.Types;
using HomeBoxLanding.Api.Features.WebSockets.Types;
using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.Radarr;

public class RadarrService : ISubscriber
{
    private readonly LinksService _linksService;
    private bool _isStarted = false;
    private const string API_KEY = "35500ce74a1c43b78b7b0e38a73fea88";

    public RadarrService(LinksService linksService)
    {
        _linksService = linksService;
    }

    public RadarrActivityResponse GetActivity()
    {
        var link = _linksService.GetAllLinks().Links.FirstOrDefault(x => x.Name.ToUpper().Contains("RADARR"));

        if (link == null)
        {
            return new RadarrActivityResponse();
        }

        var totalMovies = GetTotalMovies(link);
        
        var totalQueue = GetTotalQueue(link);
        
        var health = GetHealth(link);

        if (totalMovies == null)
        {
            return new RadarrActivityResponse();
        }

        return new RadarrActivityResponse
        {
            TotalNumberOfMovies = totalMovies.Count,
            TotalNumberOfQueuedMovies = totalQueue.Total,
            TotalMissingMovies = totalMovies.Count(x => x.SizeOnDisk == 0),
            Health = health
        };
    }

    private List<RadarrMovie> GetTotalMovies(Link link)
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(20);
        var result = httpClient.GetAsync($"{link.Url}api/v3/movie?apiKey={API_KEY}").Result;
        var response = result.Content.ReadAsStringAsync().Result;

        List<RadarrMovie>? parsedResponse;
        
        try
        {
            parsedResponse = JsonConvert.DeserializeObject<List<RadarrMovie>>(response);
        }
        catch (Exception)
        {
            return new List<RadarrMovie>();
        }

        return parsedResponse ?? new List<RadarrMovie>();
    }

    private RadarrQueue GetTotalQueue(Link link)
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(20);
        var result = httpClient.GetAsync($"{link.Url}api/v3/queue?apiKey={API_KEY}").Result;
        var response = result.Content.ReadAsStringAsync().Result;

        RadarrQueue? parsedResponse;
        
        try
        {
            parsedResponse = JsonConvert.DeserializeObject<RadarrQueue>(response);
        }
        catch (Exception)
        {
            return new RadarrQueue();
        }

        return parsedResponse ?? new RadarrQueue();
    }

    private List<RadarrHealth> GetHealth(Link link)
    {
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(20);
        var result = httpClient.GetAsync($"{link.Url}api/v3/health?apiKey={API_KEY}").Result;
        var response = result.Content.ReadAsStringAsync().Result;

        List<RadarrHealth>? parsedResponse;
        
        try
        {
            parsedResponse = JsonConvert.DeserializeObject<List<RadarrHealth>>(response);
        }
        catch (Exception)
        {
            return new List<RadarrHealth>();
        }

        return parsedResponse ?? new List<RadarrHealth>();
    }

    public void OnStarted()
    {
        _isStarted = true;

        Task.Run(() =>
        {
            while (_isStarted)
            {
                var activity = GetActivity();    
                    
                WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.RadarrActivity, new
                {
                    Response = new
                    {
                        Data = new
                        {
                            TotalNumberOfMovies = activity.TotalNumberOfMovies,
                            TotalNumberOfQueuedMovies = activity.TotalNumberOfQueuedMovies,
                            TotalMissingMovies = activity.TotalMissingMovies,
                            Health = activity.Health.ConvertAll(x => new
                            {
                                Source = x.Source,
                                Type = x.Type,
                                Message = x.Message,
                                WikiUrl = x.WikiUrl
                            })
                        }
                    }
                });
                
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