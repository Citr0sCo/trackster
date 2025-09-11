using Trackster.Api.Features.Auth.Types;

namespace Trackster.Api.Features.Auth.Providers;

public class NullAuthProvider : IAuthProvider
{
    public bool IsActive { get; } = false;
    
    public Task<SignInResponse> SignIn(SignInRequest request)
    {
        throw new NotImplementedException();
    }

    public Task<RegisterResponse> Register(RegisterRequest request)
    {
        throw new NotImplementedException();
    }
}