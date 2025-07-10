using Trackster.Api.Features.Authentication.Types;

namespace Trackster.Api.Features.Authentication.Providers;

public class NullAuthProvider : IAuthProvider
{
    public bool IsActive { get; } = false;
    
    public Task<SignInResponse> SignIn(SignInRequest request)
    {
        throw new NotImplementedException();
    }
}