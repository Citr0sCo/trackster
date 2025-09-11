using Trackster.Api.Features.Auth.Types;

namespace Trackster.Api.Features.Auth.Providers;

public interface IAuthProvider
{
    bool IsActive { get; }
    Task<SignInResponse> SignIn(SignInRequest request);
    Task<RegisterResponse> Register(RegisterRequest request);
}