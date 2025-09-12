using Trackster.Api.Data.Records;

namespace Trackster.Api.Features.Webhooks.Types;

public class CreateWebhookRequest
{
    public WebhookProvider Provider { get; set; }
    public Guid UserIdentifier { get; set; }
}