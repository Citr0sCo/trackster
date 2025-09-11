namespace Trackster.Api.Features.Auth.Types;

public class RegisterRequest
{
    public Provider Provider { get; set; }
    public string? Username { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
}