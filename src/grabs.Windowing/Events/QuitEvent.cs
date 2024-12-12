namespace grabs.Windowing.Events;

public struct QuitEvent : IWindowEvent
{
    public EventType Type => EventType.Quit;
    
    public Window Window { get; internal set; }
}