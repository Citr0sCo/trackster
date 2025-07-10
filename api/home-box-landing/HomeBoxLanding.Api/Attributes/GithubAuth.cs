using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace HomeBoxLanding.Api.Attributes;

public class GithubAuth : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext actionContext)
    {
        actionContext.HttpContext.Request.Headers.TryGetValue("X-Github-Token", out var authorizationToken);

        if (authorizationToken != "k!iKv#6958if5wufBFvD")
            actionContext.Result = new UnauthorizedResult();

        actionContext.HttpContext.Request.Headers.TryGetValue("X-Github-Event", out var webhookType);

        if (webhookType != "Pipeline Hook")
            actionContext.Result = new UnauthorizedResult();
    }
}