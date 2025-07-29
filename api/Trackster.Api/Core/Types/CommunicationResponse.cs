using Microsoft.AspNetCore.Mvc;

namespace Trackster.Api.Core.Types;

public class CommunicationResponse : OkResult
{
    public bool HasError { get; set; }
    public Error? Error { get; set; }

    public void AddError(Error? error)
    {
        HasError = true;
        Error = error;
    }
}