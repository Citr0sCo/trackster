using HomeBoxLanding.Api.Core.Types;

namespace HomeBoxLanding.Api.Features.Links.Types;

public class ImportLinksResponse : CommunicationResponse
{
    public ImportLinksResponse()
    {
        Links = new List<Link>();
    }
    
    public List<Link> Links { get; set; }
}