using HomeBoxLanding.Api.Core.Events.Types;

namespace HomeBoxLanding.Api.Core.Events;

public static class EventBus
{
    private static readonly List<ISubscriber> _subscribers = new List<ISubscriber>();
    
    public static void Register(ISubscriber subscriber)
    {
        _subscribers.Add(subscriber);
    }

    public static void OnStarted()
    {
        foreach (var subscriber in _subscribers)
            subscriber.OnStarted();
            
        Console.WriteLine("OnStarted Event Called");
    }

    public static void OnStopping()
    {
        foreach (var subscriber in _subscribers)
            subscriber.OnStopping();
            
        Console.WriteLine("OnStopping Event Called");
    }

    public static void OnStopped()
    {
        foreach (var subscriber in _subscribers)
            subscriber.OnStopped();
            
        Console.WriteLine("OnStopped Event Called");
    }
}