using Trackster.Api.Data.Records;
using Trackster.Api.Features.Users.Types;

namespace Trackster.Api.Features.Users;

public class UserMapper
{
    public static UserRecord MapRecord(User user)
    {
        return new UserRecord
        {
            Identifier = user.Identifier,
            Email = user.Email,
            Username = user.Username,
            Password = user.Password,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt,
            ThirdPartyIntegrations = user.ThirdPartyIntegrations.Select(ThirdPartyIntegrationMapper.MapRecord).ToList()
        };
    }

    public static User Map(UserRecord record)
    {
        return new User
        {
            Identifier = record.Identifier,
            Email = record.Email,
            Username = record.Username,
            Password = record.Password,
            CreatedAt = record.CreatedAt,
            UpdatedAt = record.UpdatedAt,
            ThirdPartyIntegrations = record.ThirdPartyIntegrations.Select(ThirdPartyIntegrationMapper.Map).ToList()
        };
    }
}