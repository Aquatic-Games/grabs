namespace grabs.Windowing.Events;

public struct QuitEvent : IWindowEvent
{
    public EventType Type => EventType.Quit;
}