using HomeBoxLanding.Api.Core.Types;
using HomeBoxLanding.Api.Features.Links.Types;

namespace HomeBoxLanding.Api.Features.Links;

public class LinksService
{
    private readonly ILinksRepository _linksRepository;

    private readonly string? _bucketName;
    private readonly string? _cdnUrl;

    public LinksService(ILinksRepository linksRepository)
    {
        _linksRepository = linksRepository;
    }

    public LinksResponse GetAllLinks()
    {
        var links = _linksRepository.GetAll();

        return new LinksResponse
        {
            Links = links.ConvertAll(LinkMapper.Map)
        };
    }

    public Link? GetLinkByReference(Guid linkReference)
    {
        var link = _linksRepository.GetLinkByReference(linkReference);

        if (link == null)
            return null;

        return LinkMapper.Map(link);
    }

    public ImportLinksResponse ImportLinks(ImportLinksRequest request)
    {
        var response = new ImportLinksResponse();

        var addLinkResponse = _linksRepository.ImportLinks(request);

        if (addLinkResponse.HasError)
        {
            response.AddError(addLinkResponse.Error);
            return response;
        }

        response.Links = addLinkResponse.Links;
        return response;
    }

    public AddLinkResponse AddLink(AddLinkRequest request)
    {
        var response = new AddLinkResponse();

        var addLinkResponse = _linksRepository.AddLink(request);

        if (addLinkResponse.HasError)
        {
            response.AddError(addLinkResponse.Error);
            return response;
        }

        response.Link = addLinkResponse.Link;
        return response;
    }

    public UpdateLinkResponse UpdateLink(UpdateLinkRequest request)
    {
        var response = new UpdateLinkResponse();

        var updateLinkResponse = _linksRepository.UpdateLink(request);

        if (updateLinkResponse.HasError)
        {
            response.AddError(updateLinkResponse.Error);
            return response;
        }

        response.Link = updateLinkResponse.Link;
        return response;
    }

    public CommunicationResponse DeleteLink(Guid linkReference)
    {
        var response = new CommunicationResponse();
        
        var link = _linksRepository.GetLinkByReference(linkReference);

        if (link == null)
            return response;

        if (link.IconUrl != null)
        {
            try
            {
                File.Delete(link.IconUrl);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting icon: {e.Message}");
            }
        }

        var addLinkResponse = _linksRepository.DeleteLink(linkReference);

        if (addLinkResponse == null)
        {
            response.AddError(new Error
            {
                Code = ErrorCode.DatabaseError,
                UserMessage = "Something went wrong attempting to save a link.",
                TechnicalMessage = "Something went wrong attempting to save a link."
            });
            return response;
        }

        return response;
    }

    public async Task<UploadLinkLogoResponse> UploadLogo(Guid linkReference, IFormFileCollection request)
    {
        var response = new UploadLinkLogoResponse();

        if (request.Count == 0)
        {
            response.AddError(new Error
            {
                Code = ErrorCode.DatabaseError,
                UserMessage = "Something went wrong attempting to save a link.",
                TechnicalMessage = "Something went wrong attempting to save a link."
            });
            return response;
        }

        var existingLink = _linksRepository.GetLinkByReference(linkReference);

        if (existingLink == null)
        {
            response.AddError(new Error
            {
                Code = ErrorCode.DatabaseError,
                UserMessage = "Something went wrong attempting to save a link.",
                TechnicalMessage = "Something went wrong attempting to save a link."
            });
            return response;
        }
        
        var file = request.First();

        var newFileLink = string.Empty;

        using (var stream = file.OpenReadStream())
        {
            using (var ms = new MemoryStream())
            {
                await stream.CopyToAsync(ms);
                var fileByteArray = ms.ToArray();

                Directory.CreateDirectory("assets/apps");            

                var fileLocation = $"assets/apps/{Guid.NewGuid()}{Path.GetExtension(file.FileName)}";    
                await File.WriteAllBytesAsync(fileLocation, fileByteArray);

                newFileLink = fileLocation;
            }
        }

        if (newFileLink != string.Empty && existingLink.IconUrl != null)
        {
            try
            {
                File.Delete(existingLink.IconUrl);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error deleting old icon: {e.Message}");
            }
        }

        existingLink.IconUrl = newFileLink;

        var updateLinkResponse = _linksRepository.UpdateLink(new UpdateLinkRequest
        {
            Link = LinkMapper.Map(existingLink)
        });

        if (updateLinkResponse == null)
        {
            response.AddError(new Error
            {
                Code = ErrorCode.DatabaseError,
                UserMessage = "Something went wrong attempting to save a link.",
                TechnicalMessage = "Something went wrong attempting to save a link."
            });
            return response;
        }

        response.IconUrl = existingLink.IconUrl;
        return response;
    }
}