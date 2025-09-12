using Microsoft.EntityFrameworkCore;
using Trackster.Api.Data;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Webhooks.Types;
using Guid = System.Guid;

namespace Trackster.Api.Features.Webhooks;

public interface IWebhookRepository
{
    Task<List<WebhookRecord>> GetWebhooksForUser(Guid userReference);
    Task<WebhookRecord?> GetWebhookByApiKey(string apiKey);
    Task<WebhookRecord?> GetWebhook(Guid reference);
    Task<WebhookRecord> Create(WebhookModel webhook);
}

public class WebhooksRepository : IWebhookRepository
{
    public async Task<List<WebhookRecord>> GetWebhooksForUser(Guid userReference)
    {
        await using var context = new DatabaseContext();
        
        try
        {
            return context.Webhooks
                .Include((x) => x.User)
                .Where(x => x.User.Identifier.ToString().ToUpper() == userReference.ToString().ToUpper())
                .ToList();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"[FATAL] - Failed to get webhooks.");
            Console.WriteLine(exception);
            return new List<WebhookRecord>();
        }
    }
    
    public async Task<WebhookRecord?> GetWebhookByApiKey(string apiKey)
    {
        await using var context = new DatabaseContext();
        
        try
        {
            return context.Webhooks
                .Include((x) => x.User)
                .FirstOrDefault(x => x.ApiKey.ToUpper() == apiKey.ToUpper());
        }
        catch (Exception exception)
        {
            Console.WriteLine($"[FATAL] - Failed to get webhook by api key.");
            Console.WriteLine(exception);
            return null;
        }
    }
    
    public async Task<WebhookRecord?> GetWebhook(Guid reference)
    {
        await using var context = new DatabaseContext();
        
        try
        {
            return context.Webhooks
                .Include((x) => x.User)
                .FirstOrDefault(x => x.Identifier.ToString().ToUpper() == reference.ToString().ToUpper());
        }
        catch (Exception exception)
        {
            Console.WriteLine($"[FATAL] - Failed to get webhook by reference.");
            Console.WriteLine(exception);
            return null;
        }
    }

    public async Task<WebhookRecord> Create(WebhookModel webhook)
    {
        await using var context = new DatabaseContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            var webhookRecord =  context.Webhooks.FirstOrDefault(x => x.Identifier.ToString().ToUpper() == webhook.Identifier.ToString().ToUpper());

            if (webhookRecord != null)
                return webhookRecord;
                
            var userRecord = context.Users.FirstOrDefault(x => x.Identifier.ToString().ToUpper() == webhook.UserIdentifier.ToString().ToUpper());
                
            if (userRecord == null)
                throw new Exception("User not found");
                
            webhookRecord = new WebhookRecord 
            { 
                Identifier = Guid.NewGuid(),
                ApiKey = Guid.NewGuid().ToString(),
                User = userRecord,
                Provider = webhook.Provider
            };
                
            context.Add(webhookRecord);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return webhookRecord;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"[FATAL] - Failed to create a webhook.");
            Console.WriteLine(exception);
            await transaction.RollbackAsync();
                
            throw;
        }
    }
}