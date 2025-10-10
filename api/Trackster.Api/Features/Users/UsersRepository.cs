using Microsoft.EntityFrameworkCore;
using Trackster.Api.Data;
using Trackster.Api.Data.Records;

namespace Trackster.Api.Features.Users;

public interface IUsersRepository
{
    Task<UserRecord?> GetUserByUsername(string username);
    Task<UserRecord?> GetUserByEmail(string email);
    Task<UserRecord?> GetUserByReference(Guid reference);
    Task<UserRecord> CreateUser(UserRecord user);
    Task<UserRecord> UpdateUser(UserRecord user);
}

public class UsersRepository : IUsersRepository
{
    public async Task<UserRecord?> GetUserByUsername(string username)
    {
        await using (var context = new DatabaseContext())
        {
            try
            {
                return context.Users
                    .Include(x => x.ThirdPartyIntegrations)
                    .FirstOrDefault(x => x.Username.ToUpper() == username.ToUpper());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }
    }

    public async Task<UserRecord?> GetUserByEmail(string email)
    {
        await using (var context = new DatabaseContext())
        {
            try
            {
                return context.Users
                    .Include(x => x.ThirdPartyIntegrations)
                    .FirstOrDefault(x => x.Email.ToUpper() == email.Trim().ToUpper());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }
    }

    public async Task<UserRecord?> GetUserByReference(Guid reference)
    {
        await using (var context = new DatabaseContext())
        {
            try
            {
                return context.Users
                    .Include(x => x.ThirdPartyIntegrations)
                    .FirstOrDefault(x => x.Identifier.ToString().ToUpper() == reference.ToString().ToUpper());
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                return null;
            }
        }
    }

    public async Task<UserRecord> CreateUser(UserRecord user)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var userRecord =  context.Users.FirstOrDefault(x => x.Username.ToUpper() == user.Username.ToUpper());

                if (userRecord != null)
                    return userRecord;
                
                context.Add(user);

                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return user;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                await transaction.RollbackAsync();

                return user;
            }
        }
    }

    public async Task<UserRecord> UpdateUser(UserRecord user)
    {
        using (var context = new DatabaseContext())
        using (var transaction = await context.Database.BeginTransactionAsync())
        {
            try
            {
                var existingUser = context.Users
                    .Include(x => x.ThirdPartyIntegrations)
                    .FirstOrDefault(x => x.Identifier.ToString().ToUpper() == user.Identifier.ToString().ToUpper());

                if (existingUser == null)
                    return user;
                
                if(!string.IsNullOrEmpty(user.Email) && existingUser.Email != user.Email)
                    existingUser.Email = user.Email;
                
                if(!string.IsNullOrEmpty(user.Username) && existingUser.Username != user.Username)
                    existingUser.Username = user.Username;

                context.Users.Update(existingUser);

                foreach (var integration in existingUser.ThirdPartyIntegrations)
                {
                    context.ThirdPartyIntegrations.Remove(integration);
                }

                foreach (var integration in user.ThirdPartyIntegrations)
                {
                    existingUser.ThirdPartyIntegrations.Add(integration);
                    context.ThirdPartyIntegrations.Add(integration);
                }
                
                await context.SaveChangesAsync();
                await transaction.CommitAsync();

                return existingUser;
            }
            catch (Exception exception)
            {
                Console.WriteLine(exception);
                await transaction.RollbackAsync();

                return user;
            }
        }
    }
}