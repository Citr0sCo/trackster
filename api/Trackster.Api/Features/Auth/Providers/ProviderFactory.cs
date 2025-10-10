using Trackster.Api.Features.Auth.Providers.Email;
using Trackster.Api.Features.Auth.Providers.Trakt;
using Trackster.Api.Features.Auth.Types;
using Trackster.Api.Features.Sessions;
using Trackster.Api.Features.Users;

namespace Trackster.Api.Features.Auth.Providers;

public class ProviderFactory
{
    public static IAuthProvider For(Provider provider)
    {
        if (provider == Provider.Email)
            return new EmailAuthProvider(new UsersService(new UsersRepository()), new SessionService(new SessionRepository()));
        
        if (provider == Provider.Trakt)
            return new TraktAuthProvider(new UsersService(new UsersRepository()), new SessionService(new SessionRepository()));
        
        return new NullAuthProvider();;
    }
}