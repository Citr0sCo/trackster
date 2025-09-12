namespace Trackster.Api.Features.WebSockets.Types;

public class CommonSocketMessageRequest
{
    public string? Key { get; set; }
    public CommonSockeData Data { get; set; }
    public Guid? SessionId { get; set; }
}

public class CommonSockeData
{
    public string Test { get; set; }
    public Guid UserReference { get; set; }
}