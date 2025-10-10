using Trackster.Api.Features.Auth.Types;

namespace Trackster.Api.Features.Users.Types;

public class ThirdPartyIntegration
{
    public Guid Identifier { get; set; }
    public Provider Provider { get; set; }
    public string Token { get; set; }
    public string RefreshToken { get; set; }
    public DateTime ExpiresAt { get; set; }
}