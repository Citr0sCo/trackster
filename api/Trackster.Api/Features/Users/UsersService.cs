using Trackster.Api.Data.Records;

namespace Trackster.Api.Features.Users;

public interface IUsersService
{
    Task<UserRecord?> GetUserByUsername(string username);
    Task<UserRecord> CreateUser(UserRecord user);
}

public class UsersService : IUsersService
{
    private readonly IUsersRepository _repository;

    public UsersService(IUsersRepository repository)
    {
        _repository = repository;
    }
    
    public async Task<UserRecord?> GetUserByUsername(string username)
    {
        return await _repository.GetUserByUsername(username);
    }

    public async Task<UserRecord?> GetUserByEmail(string email)
    {
        return await _repository.GetUserByEmail(email);
    }

    public async Task<UserRecord> CreateUser(UserRecord user)
    {
        return await _repository.CreateUser(user);
    }
}