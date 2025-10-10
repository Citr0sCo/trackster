using Trackster.Api.Data.Records;
using Trackster.Api.Features.Users.Types;

namespace Trackster.Api.Features.Users;

public class ThirdPartyIntegrationMapper
{
    public static ThirdPartyIntegration Map(ThirdPartyIntegrationRecord record)
    {
        return new ThirdPartyIntegration
        {
            Identifier = record.Identifier,
            Provider = record.Provider,
            Token = record.Token,
            RefreshToken = record.RefreshToken,
            ExpiresAt = record.ExpiresAt
        };
    }
    
    public static ThirdPartyIntegrationRecord MapRecord(ThirdPartyIntegration record)
    {
        return new ThirdPartyIntegrationRecord
        {
            Identifier = record.Identifier,
            Provider = record.Provider,
            Token = record.Token,
            RefreshToken = record.RefreshToken,
            ExpiresAt = record.ExpiresAt
        };
    }
}