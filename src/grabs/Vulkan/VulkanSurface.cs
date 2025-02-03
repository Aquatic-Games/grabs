using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Vulkan;

internal sealed unsafe class VulkanSurface : Surface
{
    private VkInstance _instance;
    
    private KhrSurface _khrSurface;
    private KhrWin32Surface? _win32Surface;
    private KhrXlibSurface? _xlibSurface;
    private KhrXcbSurface? _xcbSurface;
    private KhrWaylandSurface? _waylandSurface;
    
    public readonly SurfaceKHR Surface;

    public VulkanSurface(Vk vk, VkInstance instance, KhrSurface khrSurface, ref readonly SurfaceInfo info)
    {
        _instance = instance;
        _khrSurface = khrSurface;

        switch (info.Type)
        {
            case SurfaceType.Windows:
            {
                Win32SurfaceCreateInfoKHR surfaceInfo = new Win32SurfaceCreateInfoKHR()
                {
                    SType = StructureType.Win32SurfaceCreateInfoKhr,
                    Hinstance = info.Display.Windows,
                    Hwnd = info.Window.Windows
                };

                if (!vk.TryGetInstanceExtension(instance, out _win32Surface))
                    throw new Exception("Failed to get Win32 extension.");

                GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Source.General, "Creating Win32 surface.");
                _win32Surface!.CreateWin32Surface(instance, &surfaceInfo, null, out Surface)
                    .Check("Create Win32 surface");
                
                break;
            }
            case SurfaceType.Xlib:
            {
                IntPtr display = info.Display.Xlib;

                XlibSurfaceCreateInfoKHR surfaceInfo = new XlibSurfaceCreateInfoKHR()
                {
                    SType = StructureType.XlibSurfaceCreateInfoKhr,
                    Dpy = &display,
                    Window = info.Window.Xlib
                };

                if (!vk.TryGetInstanceExtension(instance, out _xlibSurface))
                    throw new Exception("Failed to get Xlib extension.");

                GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Source.General, "Creating Xlib surface.");
                _xlibSurface!.CreateXlibSurface(instance, &surfaceInfo, null, out Surface)
                    .Check("Create Xlib surface");
                
                break;
            }
            case SurfaceType.Xcb:
            {
                IntPtr connection = info.Display.Xcb;

                XcbSurfaceCreateInfoKHR xcbSurface = new XcbSurfaceCreateInfoKHR()
                {
                    SType = StructureType.XcbSurfaceCreateInfoKhr,
                    Connection = &connection,
                    Window = info.Window.Xcb
                };

                if (!vk.TryGetInstanceExtension(instance, out _xcbSurface))
                    throw new Exception("Failed to get XCB extension.");

                GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Source.General, "Creating XCB surface.");
                _xcbSurface!.CreateXcbSurface(instance, &xcbSurface, null, out Surface)
                    .Check("Create XCB surface");
                
                break;
            }
            case SurfaceType.Wayland:
            {
                IntPtr display = info.Display.Wayland;
                IntPtr surface = info.Window.Wayland;

                WaylandSurfaceCreateInfoKHR waylandSurface = new WaylandSurfaceCreateInfoKHR()
                {
                    SType = StructureType.WaylandSurfaceCreateInfoKhr,
                    Display = &display,
                    Surface = &surface
                };

                if (!vk.TryGetInstanceExtension(instance, out _waylandSurface))
                    throw new Exception("Failed to get Wayland extension.");

                GrabsLog.Log(GrabsLog.Severity.Verbose, GrabsLog.Source.General, "Creating Wayland surface.");
                _waylandSurface!.CreateWaylandSurface(instance, &waylandSurface, null, out Surface)
                    .Check("Create Wayland surface");
                
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override void Dispose()
    {
        _khrSurface.DestroySurface(_instance, Surface, null);
        
        _waylandSurface?.Dispose();
        _xcbSurface?.Dispose();
        _xlibSurface?.Dispose();
        _win32Surface?.Dispose();
    }
}