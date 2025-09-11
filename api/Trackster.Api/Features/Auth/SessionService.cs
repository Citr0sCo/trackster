using Trackster.Api.Features.Auth.Types;

namespace Trackster.Api.Features.Auth;

public class SessionService
{
    private readonly ISessionRepository _sessionRepository;

    public SessionService(ISessionRepository sessionRepository)
    {
        _sessionRepository = sessionRepository;
    }
    
    public Session CreateSession(Guid userReference)
    {
        return new Session(Guid.NewGuid(), userReference);
    }

    public async Task RemoveSession(Guid reference)
    {
        await _sessionRepository.RemoveSession(reference);
    }
}