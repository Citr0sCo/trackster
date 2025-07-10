using Trackster.Api.Features.Authentication.Providers.Trakt;
using Trackster.Api.Features.Authentication.Types;

namespace Trackster.Api.Features.Authentication.Providers;

public class ProviderFactory
{
    public static IAuthProvider For(Provider provider)
    {
        if (provider == Provider.Trakt)
            return new TraktAuthProvider();
        
        return null;
    }
}