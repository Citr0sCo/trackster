using HomeBoxLanding.Api.Features.Links.Types;

namespace HomeBoxLanding.Api.Features.Links;

public class LinkMapper
{
    public static Link Map(LinkRecord record)
    {
        return new Link
        {
            Identifier = record.Identifier,
            Name = record.Name,
            IconUrl = record.IconUrl,
            IsSecure = record.IsSecure,
            Port = record.Port,
            Host = record.Host,
            Url = record.Url,
            Category = record.Category,
            SortOrder = record.SortOrder
        };
    }
}