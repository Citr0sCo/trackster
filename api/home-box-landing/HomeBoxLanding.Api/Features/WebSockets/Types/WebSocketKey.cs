namespace HomeBoxLanding.Api.Features.WebSockets.Types;

public enum WebSocketKey
{
    Unknown,
    Handshake,
    ServerStats,
    PlexActivity,
    PiHoleActivity,
    RadarrActivity,
    SonarrActivity,
    DockerAppUpdateProgress
}