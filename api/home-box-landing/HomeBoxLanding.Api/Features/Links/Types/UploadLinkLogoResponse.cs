using HomeBoxLanding.Api.Core.Types;

namespace HomeBoxLanding.Api.Features.Links.Types;

public class UploadLinkLogoResponse : CommunicationResponse
{
    public string? IconUrl { get; set; }
}