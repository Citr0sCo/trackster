using Trackster.Api.Data.Records;

namespace Trackster.Api.Features.Webhooks.Types;

public class WebhookModel
{
    public Guid Identifier { get; set; }
    public string ApiKey { get; set; }
    public WebhookProvider Provider { get; set; }
    public Guid UserIdentifier { get; set; }
    public string Url { get; set; }
}