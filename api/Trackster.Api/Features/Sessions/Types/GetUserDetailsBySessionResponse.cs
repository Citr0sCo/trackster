using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Sessions.Types;

public class GetUserDetailsBySessionResponse : CommunicationResponse
{
    public User User { get; set; }
}