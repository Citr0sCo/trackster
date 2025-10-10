namespace Trackster.Api.Features.Auth.Types;

public class SignInRequest
{
    public Provider Provider { get; set; }
    public string? Code { get; set; }
    public string? Email { get; set; }
    public string? Password { get; set; }
    public bool Remember { get; set; }
    public Guid? UserIdentifier { get; set; }
}