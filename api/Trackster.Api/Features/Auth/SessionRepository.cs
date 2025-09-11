using Trackster.Api.Data;
using Trackster.Api.Features.Auth.Types;

namespace Trackster.Api.Features.Auth;

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
        await using (var context = new DatabaseContext())
        {
            try
            {
                return context.Sessions.FirstOrDefault(x => x.Identifier == reference);
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }
    }

    public async Task<SessionRecord> CreateSession(Session session)
    {
        
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var sessionRecord =  context.Sessions.FirstOrDefault(x => x.Identifier == session.Reference());

                if (sessionRecord != null)
                    return sessionRecord;
                
                var userRecord = context.Users.FirstOrDefault(x => x.Identifier == session.UserIdentifier());
                
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
                Console.WriteLine(exception);
                await transaction.RollbackAsync();
                
                throw;
            }
        }
    }

    public async Task<SessionRecord> ExtendSession(Session session)
    {
        
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var sessionRecord =  context.Sessions.FirstOrDefault(x => x.Identifier == session.Reference());

                if (sessionRecord == null)
                    throw new Exception("Session not found");
                
                var userRecord = context.Users.FirstOrDefault(x => x.Identifier == session.UserIdentifier());
                
                if (userRecord == null)
                    throw new Exception("User not found");

                session.ExtendTimeToLive();
                
                sessionRecord = new SessionRecord { Identifier = session.Reference(), TimeToLive = session.TimeToLive(), User = userRecord };
                
                context.Update(sessionRecord);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return sessionRecord;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                await transaction.RollbackAsync();
                
                throw;
            }
        }
    }

    public async Task RemoveSession(Guid reference)
    {
        
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var sessionRecord =  context.Sessions.FirstOrDefault(x => x.Identifier == reference);

                if (sessionRecord == null)
                    return;
                
                context.Remove(sessionRecord);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}