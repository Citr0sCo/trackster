using Trackster.Api.Features.Auth.Types;

namespace Trackster.Api.Features.Auth;

public class SessionFactory
{
    private static SessionFactory _instance;
    private readonly Dictionary<Guid, Session> _sessions = new Dictionary<Guid, Session>(); 

    private SessionFactory()
    {
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

    public bool HasSession(Guid reference)
    {
        return _sessions.ContainsKey(reference);
    }

    public void RemoveSession(Guid reference)
    {
        _sessions.Remove(reference);
    }
}