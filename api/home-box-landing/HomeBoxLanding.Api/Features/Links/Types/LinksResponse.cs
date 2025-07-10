namespace HomeBoxLanding.Api.Features.Links.Types;

public class LinksResponse
{
    public LinksResponse()
    {
        Links = new List<Link>();
    }
        
    public List<Link> Links { get; set; }
}