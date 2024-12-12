using System.Runtime.InteropServices;

namespace grabs.Windowing.Events;

[StructLayout(LayoutKind.Explicit, Size = 12)]
public readonly struct WindowEvent
{
    [FieldOffset(0)]
    public readonly Window Window;
    
    [FieldOffset(8)]
    public readonly EventType Type;

    public WindowEvent(EventType type, Window window)
    {
        Type = type;
        Window = window;
    }
}