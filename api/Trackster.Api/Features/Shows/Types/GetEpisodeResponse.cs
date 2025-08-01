using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Shows.Types;

public class GetEpisodeResponse : CommunicationResponse
{
    public Episode Episode { get; set; }
}