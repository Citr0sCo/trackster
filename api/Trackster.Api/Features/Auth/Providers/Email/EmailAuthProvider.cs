using Trackster.Api.Core.Helpers;
using Trackster.Api.Core.Types;
using Trackster.Api.Data.Records;
using Trackster.Api.Features.Auth.Types;
using Trackster.Api.Features.Sessions;
using Trackster.Api.Features.Users;

namespace Trackster.Api.Features.Auth.Providers.Email;

public class EmailAuthProvider : IAuthProvider
{
    private readonly IUsersService _usersService;
    private readonly SessionService _sessionService;
    private readonly SessionFactory _sessionFactory;
    public bool IsActive { get; } = true;

    public EmailAuthProvider(IUsersService usersService, SessionService sessionService)
    {
        _usersService = usersService;
        _sessionService = sessionService;
        _sessionFactory = SessionFactory.Instance();
    }
    
    public async Task<SignInResponse> SignIn(SignInRequest request)
    {
        if (request.Email == null)
        {
            return new SignInResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "Email is required",
                }
            };
        }
        
        if (request.Password == null)
        {
            return new SignInResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "Password is required",
                }
            };
        }
        
        var existingUser = await _usersService.GetUserByEmail(request.Email);
        
        if (existingUser == null)
        {
            return new SignInResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "User not found",
                }
            };
        }

        if (!PasswordHasher.VerifyPassword(request.Password!, existingUser.Password))
        {
            return new SignInResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "Incorrect password",
                }
            };
        }

        var session = await _sessionService.CreateSession(existingUser.Identifier, request.Remember);
        _sessionFactory.AddSession(session.Reference(), session);

        return new SignInResponse
        {
            SessionId = session.Reference(),
        };
    }

    public async Task<RegisterResponse> Register(RegisterRequest request)
    {
        if (request.Username == null)
        {
            return new RegisterResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "Username is required",
                }
            };
        }
        
        if (request.Email == null)
        {
            return new RegisterResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "Email is required",
                }
            };
        }
        
        if (request.Password == null)
        {
            return new RegisterResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "Email is required",
                }
            };
        }
        
        var existingUserByEmail = await _usersService.GetUserByEmail(request.Email);
        
        if (existingUserByEmail != null)
        {
            return new RegisterResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "Email already in use",
                }
            };
        }
        
        var existingUserByUsername = await _usersService.GetUserByUsername(request.Username);
        
        if (existingUserByUsername != null)
        {
            return new RegisterResponse
            {
                HasError = true,
                Error = new Error
                {
                    UserMessage = "Username already in use",
                }
            };
        }

        var userRecord = new UserRecord
        {
            Identifier = Guid.NewGuid(),
            Email = request.Email,
            Username = request.Username,
            Password = PasswordHasher.HashPassword(request.Password),
            CreatedAt = DateTime.Now,
            UpdatedAt = DateTime.Now,
        };
        
        var createdUsed = await _usersService.CreateUser(userRecord);

        var session = await _sessionService.CreateSession(createdUsed.Identifier);
        _sessionFactory.AddSession(session.Reference(), session);

        return new RegisterResponse
        {
            SessionId = session.Reference(),
        };
    }
}