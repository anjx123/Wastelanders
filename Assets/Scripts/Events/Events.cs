public interface IEvent { }
public static class EventExtensions
{
    // Allows you to just call an event via .Invoke() instead of typing out the entire event bus.
    public static void Invoke<TEvent>(this TEvent anEvent) where TEvent : IEvent => EventBus.Raise(anEvent);
}