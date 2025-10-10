using Trackster.Api.Features.Settings.Types;

namespace Trackster.Api.Features.Settings;

public interface ISettingsService
{
    GetSettingsResponse GetSettings(Guid getSessionId);
}

public class SettingsService : ISettingsService
{
    private readonly ISettingsRepository _repository;

    public SettingsService(ISettingsRepository repository)
    {
        _repository = repository;
    }
    
    public GetSettingsResponse GetSettings(Guid getSessionId)
    {
        return new GetSettingsResponse
        {
            Settings = new Types.Settings
            {
                TraktClientId = Environment.GetEnvironmentVariable("ASPNETCORE_TRAKT_CLIENT_ID")!
            }
        };
    }
}