using Trackster.Api.Data;
using Trackster.Api.Data.Records;

namespace Trackster.Api.Features.Users;

public interface IUsersRepository
{
    Task<UserRecord?> GetUserByUsername(string username);
    Task<UserRecord> CreateUser(UserRecord user);
}

public class UsersRepository : IUsersRepository
{
    public async Task<UserRecord?> GetUserByUsername(string username)
    {
        await using (var context = new DatabaseContext())
        {
            try
            {
                return context.Users.FirstOrDefault(x => x.Username.ToUpper() == username.ToUpper());
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
}