using Trackster.Api.Data.Records;
using Trackster.Api.Features.Webhooks.Types;
using Guid = System.Guid;

namespace Trackster.Api.Features.Webhooks;

public class WebhooksService
{
    private readonly IWebhookRepository _webhookRepository;

    public WebhooksService(IWebhookRepository webhookRepository)
    {
        _webhookRepository = webhookRepository;
    }

    public async Task<WebhookModel?> GetWebhookByApiKey(string apiKey)
    {
        var webhookRecord = await _webhookRepository.GetWebhookByApiKey(apiKey);

        if (webhookRecord != null)
            return new WebhookModel
            {
                Identifier = webhookRecord.Identifier,
                ApiKey = webhookRecord.ApiKey,
                Provider = webhookRecord.Provider,
                UserIdentifier = webhookRecord.User.Identifier
            };

        return null;
    }

    public async Task<WebhookModel?> GetWebhookForUser(Guid reference)
    {
        var webhookRecord = await _webhookRepository.GetWebhooksForUser(reference);

        if (webhookRecord.Count > 0)
        {
            return new WebhookModel
            {
                Identifier = webhookRecord[0].Identifier,
                ApiKey = webhookRecord[0].ApiKey,
                Provider = webhookRecord[0].Provider,
                UserIdentifier = webhookRecord[0].User.Identifier,
                Url = $"{Environment.GetEnvironmentVariable("ASPNETCORE_BASE_URL")}/api/webhooks/{webhookRecord[0].Provider.ToString().ToLower()}/{webhookRecord[0].ApiKey}"
            };
        }

        return null;
    }

    public async Task<WebhookModel?> GetWebhook(Guid reference)
    {
        var webhookRecord = await _webhookRepository.GetWebhook(reference);

        if (webhookRecord != null)
            return new WebhookModel
            {
                Identifier = webhookRecord.Identifier,
                ApiKey = webhookRecord.ApiKey,
                Provider = webhookRecord.Provider,
                UserIdentifier = webhookRecord.User.Identifier
            };

        return null;
    }

    public async Task<WebhookModel> CreateWebhook(Guid userReference, WebhookProvider provider)
    {
        var webhook = new WebhookModel
        {
            UserIdentifier = userReference,
            Provider = provider
        };

        var webhookRecord = await _webhookRepository.Create(webhook);

        return new  WebhookModel
        {
            Identifier = webhookRecord.Identifier,
            ApiKey = webhookRecord.ApiKey,
            Provider = webhookRecord.Provider,
            UserIdentifier = webhookRecord.User.Identifier,
            Url = $"{Environment.GetEnvironmentVariable("ASPNETCORE_BASE_URL")}/api/webhooks/{webhookRecord.Provider.ToString().ToLower()}/{webhookRecord.ApiKey}"
        };
    }
}