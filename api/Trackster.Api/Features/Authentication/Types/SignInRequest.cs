namespace Trackster.Api.Features.Authentication.Types;

public class SignInRequest
{
    public Provider Provider { get; set; }
    public string? Code { get; set; }
    public string? Username { get; set; }
    public string? Password { get; set; }
}