namespace HomeBoxLanding.Api.Features.Links.Types;

public class UpdateLinkRequest
{
    public Link? Link { get; set; }
    public bool MoveUp { get; set; }
    public bool MoveDown { get; set; }
}