using Trackster.Api.Core.Types;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Auth.Types;
using Trackster.Api.Features.Sessions;
using Trackster.Api.Features.Users.Types;

namespace Trackster.Api.Features.Users;

public interface IUsersService
{
    Task<UserRecord?> GetUserByUsername(string username);
    Task<UserRecord> CreateUser(UserRecord user);
}

public class UsersService : IUsersService
{
    private readonly IUsersRepository _repository;
    private readonly SessionFactory _sessionFactory;

    public UsersService(IUsersRepository repository)
    {
        _repository = repository;
        _sessionFactory = SessionFactory.Instance();
    }
    
    public async Task<UserRecord?> GetUserByUsername(string username)
    {
        return await _repository.GetUserByUsername(username);
    }

    public async Task<UserRecord?> GetUserByEmail(string email)
    {
        return await _repository.GetUserByEmail(email);
    }

    public async Task<GetUserDetailsResponse> GetUserByReference(Guid reference)
    {
        var user = await _repository.GetUserByReference(reference);

        if (user == null)
        {
            return new GetUserDetailsResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "User does not exist",
                }
            };
        }

        return new GetUserDetailsResponse
        {
            User = UserMapper.Map(user)
        };
    }

    public async Task<GetUserDetailsResponse> GetUserByReference(Guid sessionId, Guid reference)
    {
        var user = await _repository.GetUserByReference(reference);

        if (user == null)
        {
            return new GetUserDetailsResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "User does not exist",
                }
            };
        }

        var session = _sessionFactory.GetSession(sessionId);

        if (session?.Reference() != sessionId)
        {
            return new GetUserDetailsResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "Not authorised to view this user",
                }
            };
        }

        await _sessionFactory.ExtendSession(sessionId);

        return new GetUserDetailsResponse
        {
            User = new User
            {
               Identifier = user.Identifier,
               Username = user.Username,
               CreatedAt = user.CreatedAt,
            }
        };
    }

    public async Task<UserRecord> CreateUser(UserRecord user)
    {
        return await _repository.CreateUser(user);
    }

    public async Task<UserRecord> UpdateUser(User user)
    {
        return await _repository.UpdateUser(UserMapper.MapRecord(user));
    }
}