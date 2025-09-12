using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Users.Types;

public class GetUserDetailsResponse : CommunicationResponse
{
    public User User { get; set; }
}