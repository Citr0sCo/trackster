using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Shows.Types;

public class GetShowResponse : CommunicationResponse
{
    public Show Show { get; set; }
}