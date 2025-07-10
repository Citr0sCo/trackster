using HomeBoxLanding.Api.Core.Events.Types;
using HomeBoxLanding.Api.Features.Links;
using HomeBoxLanding.Api.Features.PiHole.Types;
using HomeBoxLanding.Api.Features.WebSockets.Types;
using Newtonsoft.Json;

namespace HomeBoxLanding.Api.Features.PiHole;

public class PiHoleService : ISubscriber
{
    private readonly LinksService _linksService;
    private bool _isStarted = false;
    private readonly Dictionary<string, string> _sessions = new Dictionary<string, string>();

    public PiHoleService(LinksService linksService)
    {
        _linksService = linksService;
    }

    public PiHoleActivityResponse GetActivity(Guid identifier)
    {
        var link = _linksService.GetAllLinks().Links.FirstOrDefault(x => x.Identifier == identifier);

        if (link == null)
            return new PiHoleActivityResponse();

        var baseUrl = $"http://{link.Host}";
        
        var sessionId = Authenticate(baseUrl);
        
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(20);
        httpClient.DefaultRequestHeaders.Add("sid", sessionId);
        var result = httpClient.GetAsync($"{baseUrl}/api/stats/summary").Result;
        var response = result.Content.ReadAsStringAsync().Result;

        PiHoleActivityResponse? parsedResponse;
        
        try
        {
            parsedResponse = JsonConvert.DeserializeObject<PiHoleActivityResponse>(response);
        }
        catch (Exception)
        {
            DeleteSession(baseUrl, sessionId);
            return new PiHoleActivityResponse();
        }

        if (parsedResponse == null)
        {
            DeleteSession(baseUrl, sessionId);
            return new PiHoleActivityResponse();
        }

        parsedResponse.Identifier = identifier;
        
        if(parsedResponse.Queries != null)
            parsedResponse.Queries.PercentBlocked = Math.Round(parsedResponse.Queries?.PercentBlocked ?? 0, 2);
        
        DeleteSession(baseUrl, sessionId);
        return parsedResponse;
    }
    
    private string? Authenticate(string baseUrl)
    {
        var request = new PiHoleAuthenticateRequest()
        {
            Password = "zZ83g5jk"
        };
        
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(20);
        var result = httpClient.PostAsync($"{baseUrl}/api/auth", new StringContent(JsonConvert.SerializeObject(request))).Result;
        var response = result.Content.ReadAsStringAsync().Result;

        PiHoleAuthenticateResponse? parsedResponse;
        
        try
        {
            parsedResponse = JsonConvert.DeserializeObject<PiHoleAuthenticateResponse>(response);
        }
        catch (Exception)
        {
            return null;
        }

        if (parsedResponse == null)
        {
            return null;
        }

        if(parsedResponse.Session != null)
            _sessions.Add(baseUrl, parsedResponse.Session.SessionId);
        
        return parsedResponse.Session?.SessionId;
    }
    
    private void DeleteSession(string baseUrl, string? sessionId)
    {
        if (sessionId == null)
            return;
        
        var httpClient = new HttpClient();
        httpClient.Timeout = TimeSpan.FromSeconds(20);
        httpClient.DefaultRequestHeaders.Add("sid", sessionId);
        var result = httpClient.DeleteAsync($"{baseUrl}/api/auth").Result;
        var response = result.Content.ReadAsStringAsync().Result;

        PiHoleAuthenticateResponse? parsedResponse;
        
        try
        {
            parsedResponse = JsonConvert.DeserializeObject<PiHoleAuthenticateResponse>(response);
        }
        catch (Exception)
        {
            return;
        }
        
        _sessions.Remove(baseUrl);
    }

    public void OnStarted()
    {
        _isStarted = true;

        Task.Run(() =>
        {
            while (_isStarted)
            {
                var linkService = new LinksService(new LinksRepository());
                var piHoleLinks = linkService.GetAllLinks().Links.Where(x => x.Name.ToUpper().Contains("PIHOLE"));
                
                var activities = new Dictionary<Guid, PiHoleActivityResponse>();
                
                foreach (var link in piHoleLinks)
                {
                    activities.Add(link.Identifier!.Value, GetActivity(link.Identifier!.Value));    
                }
                    
                WebSockets.WebSocketManager.Instance().SendToAllClients(WebSocketKey.PiHoleActivity, new
                {
                    Response = new
                    {
                        Data = new
                        {
                            Activities = activities.Select(x => new
                            {
                                Identifier = x.Value.Identifier,
                                Queries = new
                                {
                                    Total = x.Value.Queries?.Total,
                                    Blocked = x.Value.Queries?.Blocked,
                                    PercentBlocked = Math.Round(x.Value.Queries?.PercentBlocked ?? 0, 2),
                                },
                                Clients = new 
                                {
                                    Total = x.Value.Clients?.Total,
                                }
                            }).ToList()
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
        
        foreach (var session in _sessions)
        {
            DeleteSession(session.Key, session.Value);
        }
    }

    public void OnStopped()
    {
        // Do nothing
    }
}