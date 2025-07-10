using Trackster.Api.Core.Types;
using Trackster.Api.Features.Authentication.Providers;
using Trackster.Api.Features.Authentication.Types;

namespace Trackster.Api.Features.Authentication;

public class AuthenticationService
{
    private readonly IAuthenticationRepository _authenticationRepository;

    public AuthenticationService(IAuthenticationRepository authenticationRepository)
    {
        _authenticationRepository = authenticationRepository;
    }

    public async Task<SignInResponse> SignIn(SignInRequest request)
    {
        var provider = ProviderFactory.For(request.Provider);

        if (provider.IsActive)
            return await provider.SignIn(request);

        return new SignInResponse
        {
            HasError = true,
            Error = new Error
            {
                UserMessage = "No provider specified.",
                TechnicalMessage = $"Provider {request.Provider} does not exist."
            }
        };
    }
}