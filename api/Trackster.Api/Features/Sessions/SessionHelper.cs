using Trackster.Api.Features.Users;
using Trackster.Api.Features.Users.Types;

namespace Trackster.Api.Features.Sessions;

public class SessionHelper
{
    private readonly SessionService _sessionService;
    private readonly UsersService _usersService;

    public SessionHelper(SessionService sessionService, UsersService usersService)
    {
        _sessionService = sessionService;
        _usersService = usersService;
    }
    
    public Guid GetSessionId(HttpContext httpContext)
    {
        var sessionId = httpContext.Items["SessionId"];
        
        if(sessionId == null)
            return Guid.Empty;
        
        if(!Guid.TryParse(sessionId.ToString(), out var parsedSessionId))
            return Guid.Empty;
        
        return parsedSessionId;
    }

    public async Task<User?> GetUser(HttpContext httpContext)
    {
        var sessionId = GetSessionId(httpContext);

        var session = await _sessionService.GetSession(sessionId);

        if (session == null)
            return null;

        var response = await _usersService.GetUserByReference(sessionId, session.UserIdentifier());

        return response.User;
    }

    public async Task<User?> GetUser(Guid sessionToken)
    {
        var session = await _sessionService.GetSession(sessionToken);

        if (session == null)
            return null;
        
        var response = await _usersService.GetUserByReference(sessionToken, session.UserIdentifier());

        return response.User;
    }
}