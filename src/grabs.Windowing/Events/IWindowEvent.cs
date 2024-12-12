namespace grabs.Windowing.Events;

public interface IWindowEvent
{
    public EventType Type { get; }
    
    public Window Window { get; internal set; }
}