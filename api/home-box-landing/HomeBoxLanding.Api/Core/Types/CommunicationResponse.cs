namespace HomeBoxLanding.Api.Core.Types;

public class CommunicationResponse
{
    public bool HasError { get; set; }
    public Error? Error { get; set; }

    public void AddError(Error? error)
    {
        HasError = true;
        Error = error;
    }
}