using HomeBoxLanding.Api.Core.Types;

namespace HomeBoxLanding.Api.Features.Links.Types;

public class AddLinkResponse : CommunicationResponse
{
    public Link? Link { get; set; }
}