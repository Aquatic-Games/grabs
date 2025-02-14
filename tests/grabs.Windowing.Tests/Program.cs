using grabs.Core;
using grabs.Windowing;
using grabs.Windowing.Events;

WindowInfo windowInfo = new WindowInfo(new Size2D(1280, 720), "grabs.Windowing.Tests");

using Window window = new Window(in windowInfo);

bool alive = true;
while (alive)
{
    while (window.PollEvent(out Event winEvent))
    {
        switch (winEvent.Type)
        {
            case EventType.Close:
                alive = false;
                break;
        }
    }
}