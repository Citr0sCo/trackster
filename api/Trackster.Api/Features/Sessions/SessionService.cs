using Trackster.Api.Features.Auth.Types;
using Trackster.Api.Features.Sessions.Types;

namespace Trackster.Api.Features.Sessions;

public class SessionService
{
    private readonly ISessionRepository _sessionRepository;

    public SessionService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }

    public async Task<Session?> GetSession(Guid reference)
    {
        var sessionRecord = await _sessionRepository.GetSession(reference);

        if (sessionRecord != null)
            return new Session(sessionRecord);

        return null;
    }
    
    public async Task<Session> CreateSession(Guid userReference, bool remember = false)
    {
        var session = new Session(Guid.NewGuid(), userReference, remember);
        await _sessionRepository.CreateSession(session);
        
        return session;
    }

    public async Task RemoveSession(Guid reference)
    {
        await _sessionRepository.RemoveSession(reference);
    }

    public async Task ExtendSession(Session session)
    {
        await _sessionRepository.ExtendSession(session);
    }
}