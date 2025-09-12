namespace Trackster.Api.Features.Users.Types;

public class User
{
    public Guid Identifier { get; set; }
    public string Username { get; set; }
    public DateTime CreatedAt { get; set; }
}