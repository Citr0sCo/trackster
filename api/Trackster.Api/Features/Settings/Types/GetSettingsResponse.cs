using Trackster.Api.Core.Types;

namespace Trackster.Api.Features.Settings.Types;

public class GetSettingsResponse : CommunicationResponse
{
    public Settings Settings { get; set; }
}