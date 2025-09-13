using Trackster.Api.Features.Auth.Types;
using Trackster.Api.Features.Sessions.Types;

namespace Trackster.Api.Features.Sessions;

public class SessionFactory
{
    private static SessionFactory? _instance;
    private readonly Dictionary<Guid, Session> _sessions = new Dictionary<Guid, Session>();
    private readonly SessionService _sessionService;

    private SessionFactory()
    {
        _sessionService = new SessionService(new SessionRepository());
    }

    public static SessionFactory Instance()
    {
        if (_instance == null)
            _instance = new SessionFactory();
        
        return _instance;
    }

    public void AddSession(Guid reference, Session session)
    {
        _sessions.Add(reference, session);
    }

    public async Task<bool> HasSession(Guid reference)
    {
        var session = await _sessionService.GetSession(reference);

        if (session == null)
        {
            _sessions.Remove(reference);
            return false;
        }

        if (session.Expired())
        {
            _sessions.Remove(reference);
            return false;
        }

        _sessions[reference] = session;
        
        return _sessions.ContainsKey(reference);
    }

    public void RemoveSession(Guid reference)
    {
        _sessions.Remove(reference);
    }

    public Session? GetSession(Guid sessionId)
    {
        if (_sessions.TryGetValue(sessionId, out var session))
            return session;

        return null;
    }

    public async Task ExtendSession(Guid sessionId)
    {
        if (!await HasSession(sessionId))
            return;
        
        if (_sessions.TryGetValue(sessionId, out var session))
            await _sessionService.ExtendSession(session);
    }
}