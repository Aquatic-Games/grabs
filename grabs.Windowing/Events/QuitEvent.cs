namespace grabs.Windowing.Events;

public struct QuitEvent : IWindowEvent
{
    public EventType EventType => EventType.Quit;
}