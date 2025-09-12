using Microsoft.EntityFrameworkCore;
using Trackster.Api.Data;
using Trackster.Api.Features.Auth.Types;
using Trackster.Api.Features.Sessions.Types;

namespace Trackster.Api.Features.Sessions;

public interface ISessionRepository
{
    Task<SessionRecord?> GetSession(Guid reference);
    Task<SessionRecord> CreateSession(Session session);
    Task<SessionRecord> ExtendSession(Session session);
    Task RemoveSession(Guid reference);
}

public class SessionRepository : ISessionRepository
{
    public async Task<SessionRecord?> GetSession(Guid reference)
    {
        await using var context = new DatabaseContext();
        
        try
        {
            return context.Sessions
                .Include((x) => x.User)
                .FirstOrDefault(x => x.Identifier.ToString().ToUpper() == reference.ToString().ToUpper());
        }
        catch (Exception exception)
        {
            Console.WriteLine($"[FATAL] - Failed to get session.");
            Console.WriteLine(exception);
            return null;
        }
    }

    public async Task<SessionRecord> CreateSession(Session session)
    {
        await using var context = new DatabaseContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            var sessionRecord =  context.Sessions.FirstOrDefault(x => x.Identifier.ToString().ToUpper() == session.Reference().ToString().ToUpper());

            if (sessionRecord != null)
                return sessionRecord;
                
            var userRecord = context.Users.FirstOrDefault(x => x.Identifier.ToString().ToUpper() == session.UserIdentifier().ToString().ToUpper());
                
            if (userRecord == null)
                throw new Exception("User not found");
                
            sessionRecord = new SessionRecord { Identifier = session.Reference(), TimeToLive = session.TimeToLive(), User = userRecord };
                
            context.Add(sessionRecord);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return sessionRecord;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"[FATAL] - Failed to create session.");
            Console.WriteLine(exception);
            await transaction.RollbackAsync();
                
            throw;
        }
    }

    public async Task<SessionRecord> ExtendSession(Session session)
    {
        await using var context = new DatabaseContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            var sessionRecord =  context.Sessions.FirstOrDefault(x => x.Identifier.ToString().ToUpper() == session.Reference().ToString().ToUpper());

            if (sessionRecord == null)
                throw new Exception("Session not found");
                
            var userRecord = context.Users.FirstOrDefault(x => x.Identifier.ToString().ToUpper() == session.UserIdentifier().ToString().ToUpper());
                
            if (userRecord == null)
                throw new Exception("User not found");

            session.ExtendTimeToLive();
                
            sessionRecord.TimeToLive = session.TimeToLive();
                
            context.Update(sessionRecord);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();

            return sessionRecord;
        }
        catch (Exception exception)
        {
            Console.WriteLine($"[FATAL] - Failed to extend session.");
            Console.WriteLine(exception);
            await transaction.RollbackAsync();
                
            throw;
        }
    }

    public async Task RemoveSession(Guid reference)
    {
        await using var context = new DatabaseContext();
        await using var transaction = await context.Database.BeginTransactionAsync();
        
        try
        {
            var sessionRecord =  context.Sessions.FirstOrDefault(x => x.Identifier.ToString().ToUpper() == reference.ToString().ToUpper());

            if (sessionRecord == null)
                return;
                
            context.Remove(sessionRecord);

            await context.SaveChangesAsync();
            await transaction.CommitAsync();
        }
        catch (Exception exception)
        {
            Console.WriteLine($"[FATAL] - Failed to remove session.");
            Console.WriteLine(exception);
            await transaction.RollbackAsync();
            throw;
        }
    }
}