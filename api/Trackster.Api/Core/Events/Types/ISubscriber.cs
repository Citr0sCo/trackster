namespace Trackster.Api.Core.Events.Types;

public interface ISubscriber
{
    void OnStarted();
    void OnStopping();
    void OnStopped();
}