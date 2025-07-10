using Trackster.Api.Features.Authentication.Types;

namespace Trackster.Api.Features.Authentication.Providers;

public interface IAuthProvider
{
    bool IsActive { get; }
    Task<SignInResponse> SignIn(SignInRequest request);
}