using grabs.Windowing;
using grabs.Windowing.Events;

WindowInfo info = new WindowInfo("Test Window", 1280, 720);

using Window window = new Window(info);

bool alive = true;
while (alive)
{
    while (window.PollEvent(out IWindowEvent winEvent))
    {
        switch (winEvent)
        {
            case QuitEvent:
                alive = false;
                break;
        }
    }
}