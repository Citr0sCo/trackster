using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Media.Types;

public class GetMediaByIdResponse : CommunicationResponse
{
    public Media Media { get; set; }
}