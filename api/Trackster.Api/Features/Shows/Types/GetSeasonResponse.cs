using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Shows.Types;

public class GetSeasonResponse : CommunicationResponse
{
    public Season Season { get; set; }
}