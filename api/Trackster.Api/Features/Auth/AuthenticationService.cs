using Trackster.Api.Core.Types;
using Trackster.Api.Features.Auth.Providers;
using Trackster.Api.Features.Auth.Types;

namespace Trackster.Api.Features.Auth;

public class AuthenticationService
{
    private readonly SessionService _sessionService;

    public AuthenticationService(SessionService sessionService)
    {
        _sessionService = sessionService;
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

    public async Task<RegisterResponse> Register(RegisterRequest request)
    {
        var provider = ProviderFactory.For(request.Provider);

        if (provider.IsActive)
            return await provider.Register(request);

        return new RegisterResponse
        {
            HasError = true,
            Error = new Error
            {
                UserMessage = "No provider specified.",
                TechnicalMessage = $"Provider {request.Provider} does not exist."
            }
        };
    }

    public async Task<CommunicationResponse> SignOut(Guid reference)
    {
        await _sessionService.RemoveSession(reference);
        SessionFactory.Instance().RemoveSession(reference);
        
        return  new CommunicationResponse();
    }
}