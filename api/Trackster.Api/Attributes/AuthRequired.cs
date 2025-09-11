using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Trackster.Api.Features.Auth;

namespace Trackster.Api.Attributes;

public class AuthRequired : ActionFilterAttribute
{
    private readonly SessionFactory _sessionFactory;

    public AuthRequired()
    {
        _sessionFactory = SessionFactory.Instance();
    }
    
    public override void OnActionExecuting(ActionExecutingContext actionContext)
    {
        actionContext.HttpContext.Request.Headers.TryGetValue("X-Authentication-Token", out var authorizationToken);

        if(!Guid.TryParse(authorizationToken, out Guid sessionGuid))
            actionContext.Result = new UnauthorizedResult();
        
        if (!_sessionFactory.HasSession(sessionGuid))
            actionContext.Result = new UnauthorizedResult();
    }
}