using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Auth.Types;

public class SignInResponse : CommunicationResponse
{
    public Guid SessionId { get; set; }
}