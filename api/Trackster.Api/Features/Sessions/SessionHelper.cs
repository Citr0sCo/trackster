namespace Trackster.Api.Features.Sessions;

public class SessionHelper
{
    public Guid GetSessionId(HttpContext httpContext)
    {
        var sessionId = httpContext.Items["SessionId"];
        
        if(sessionId == null)
            return Guid.Empty;
        
        if(!Guid.TryParse(sessionId.ToString(), out var parsedSessionId))
            return Guid.Empty;
        
        return parsedSessionId;
    }
}