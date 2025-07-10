namespace HomeBoxLanding.Api.Features.WebSockets.Types;

public class CommonSocketMessageRequest
{
    public string? Key { get; set; }
    public object? Data { get; set; }
    public Guid? SessionId { get; set; }
}