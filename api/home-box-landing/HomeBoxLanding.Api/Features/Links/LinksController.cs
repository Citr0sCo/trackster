using HomeBoxLanding.Api.Core.Types;
using HomeBoxLanding.Api.Features.Links.Types;
using Microsoft.AspNetCore.Mvc;

namespace HomeBoxLanding.Api.Features.Links;

[ApiController]
[Route("api/[controller]")]
public class LinksController : ControllerBase
{
    private readonly LinksService _service;

    public LinksController()
    {
        _service = new LinksService(new LinksRepository());
    }

    [HttpGet]
    public LinksResponse GetAll()
    {
        return _service.GetAllLinks();
    }

    [HttpPost("import")]
    public ImportLinksResponse ImportLinks([FromBody]ImportLinksRequest request)
    {
        return _service.ImportLinks(request);
    }

    [HttpPost("{linkReference}/logo")]
    public async Task<CommunicationResponse> UploadLogo(Guid linkReference)
    {
        return await _service.UploadLogo(linkReference, Request.Form.Files);
    }

    [HttpPost("")]
    public AddLinkResponse AddLink([FromBody]AddLinkRequest request)
    {
        return _service.AddLink(request);
    }

    [HttpPatch("{linkReference}")]
    public UpdateLinkResponse UpdateLink(Guid linkReference, [FromBody]UpdateLinkRequest request)
    {
        return _service.UpdateLink(request);
    }

    [HttpDelete("{linkReference}")]
    public CommunicationResponse DeleteLink(Guid linkReference)
    {
        return _service.DeleteLink(linkReference);
    }
}