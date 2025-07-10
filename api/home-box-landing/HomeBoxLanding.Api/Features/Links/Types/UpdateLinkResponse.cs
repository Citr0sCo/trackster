using HomeBoxLanding.Api.Core.Types;

namespace HomeBoxLanding.Api.Features.Links.Types;

public class UpdateLinkResponse : CommunicationResponse
{
    public Link? Link { get; set; }
}