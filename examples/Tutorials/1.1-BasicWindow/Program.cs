using grabs.Graphics;
using grabs.Windowing;

WindowInfo windowInfo = new WindowInfo("Tutorial 1.1 - Basic Window", 1280, 720);
using Window window = new Window(windowInfo);

using Instance instance = window.CreateInstance();
using Surface surface = window.CreateSurface();

using Device device = instance.CreateDevice(surface);
