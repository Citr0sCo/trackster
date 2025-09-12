using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Trackster.Api.Features.Sessions;

namespace Trackster.Api.Attributes;

public class AuthRequired : ActionFilterAttribute
{
    private readonly SessionFactory _sessionFactory;

    public AuthRequired()
    {
        _sessionFactory = SessionFactory.Instance();
    }
    
    public override async void OnActionExecuting(ActionExecutingContext context)
    {
        context.HttpContext.Request.Headers.TryGetValue("X-Authentication-Token", out var authorizationToken);

        if(!Guid.TryParse(authorizationToken, out Guid sessionGuid))
            context.Result = new UnauthorizedResult();
        
        if (!await _sessionFactory.HasSession(sessionGuid))
            context.Result = new UnauthorizedResult();

        context.HttpContext.Items["SessionId"] = authorizationToken;
    }
}