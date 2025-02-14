namespace grabs.Windowing.Events;

public struct Event
{
    public readonly EventType Type;

    public Event(EventType type)
    {
        Type = type;
    }
}