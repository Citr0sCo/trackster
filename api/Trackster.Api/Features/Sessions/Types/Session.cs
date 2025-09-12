namespace Trackster.Api.Features.Sessions.Types;

public class User
{
    public Guid Identifier { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
}