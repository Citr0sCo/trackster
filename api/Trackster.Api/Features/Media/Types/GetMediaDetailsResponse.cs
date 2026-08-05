using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Media.Types;

public class GetMediaDetailsResponse : CommunicationResponse
{
    public MediaDetails? Details { get; set; }
}
