using Trackster.Api.Features.Authentication.Types;

namespace Trackster.Api.Features.Authentication;

public class SessionFactory
{
    private static SessionFactory _instance;
    private Dictionary<Guid, Session> _sessions = new Dictionary<Guid, Session>(); 

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
}