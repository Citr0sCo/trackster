namespace HomeBoxLanding.Api.Features.Links.Types;

public class ImportLinksRequest
{
    public ImportLinksRequest()
    {
        Links = new List<Link>();
    }
    
    public List<Link> Links { get; set; }
}