using System.Diagnostics;
using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

internal sealed unsafe class VkSurface : Surface
{
    public override bool IsDisposed { get; protected set; }

    private readonly KhrSurface _khrSurface;
    private readonly VulkanInstance _instance;
    
    private readonly KhrWin32Surface? _win32Surface;
    private readonly KhrXlibSurface? _xlibSurface;
    private readonly KhrXcbSurface? _xcbSurface;
    private readonly KhrWaylandSurface? _waylandSurface;
    
    public readonly SurfaceKHR Surface;

    public VkSurface(Vk vk, KhrSurface khrSurface, VulkanInstance instance, ref readonly SurfaceInfo info)
    {
        ResourceTracker.RegisterInstanceResource(instance, this);
        
        _khrSurface = khrSurface;
        _instance = instance;
        
        switch (info.Type)
        {
            case SurfaceType.Windows:
            {
                Win32SurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.Win32SurfaceCreateInfoKhr,
                    Hinstance = info.Display,
                    Hwnd = info.Window
                };

                if (!vk.TryGetInstanceExtension(instance, out _win32Surface))
                    throw new Exception("Failed to get Win32 surface extension.");
                
                Debug.Assert(_win32Surface != null);

                GrabsLog.Log("Creating Win32 surface.");
                _win32Surface.CreateWin32Surface(instance, &surfaceInfo, null, out Surface)
                    .Check("Create Win32 surface");
                
                break;
            }
            case SurfaceType.Xlib:
            {
                XlibSurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.XlibSurfaceCreateInfoKhr,
                    Dpy = (IntPtr*) info.Display,
                    Window = info.Window
                };

                if (!vk.TryGetInstanceExtension(instance, out _xlibSurface))
                    throw new Exception("Failed to get Xlib surface extension.");
                
                Debug.Assert(_xlibSurface != null);
                
                GrabsLog.Log("Creating xlib surface.");
                _xlibSurface.CreateXlibSurface(instance, &surfaceInfo, null, out Surface).Check("Create xlib surface");
                
                break;
            }
            case SurfaceType.Xcb:
            {
                XcbSurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.XcbSurfaceCreateInfoKhr,
                    Connection = (IntPtr*) info.Display,
                    Window = info.Window
                };

                if (!vk.TryGetInstanceExtension(instance, out _xcbSurface))
                    throw new Exception("Failed to get Xcb surface extension.");
                
                Debug.Assert(_xcbSurface != null);
                
                GrabsLog.Log("Creating xcb surface.");
                _xcbSurface.CreateXcbSurface(instance, &surfaceInfo, null, out Surface).Check("Create xcb surface");
                
                break;
            }
            case SurfaceType.Wayland:
            {
                WaylandSurfaceCreateInfoKHR surfaceInfo = new()
                {
                    SType = StructureType.WaylandSurfaceCreateInfoKhr,
                    Display = (IntPtr*) info.Display,
                    Surface = (IntPtr*) info.Window
                };

                if (!vk.TryGetInstanceExtension(instance, out _waylandSurface))
                    throw new Exception("Failed to get Wayland surface extension.");
                
                Debug.Assert(_waylandSurface != null);
                
                GrabsLog.Log("Creating Wayland surface.");
                _waylandSurface.CreateWaylandSurface(instance, &surfaceInfo, null, out Surface)
                    .Check("Create Wayland surface");
                
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;

        IsDisposed = true;
        
        GrabsLog.Log("Destroying surface.");
        _khrSurface.DestroySurface(_instance, Surface, null);
        
        _waylandSurface?.Dispose();
        _xcbSurface?.Dispose();
        _xlibSurface?.Dispose();
        _win32Surface?.Dispose();
        
        ResourceTracker.DeregisterInstanceResource(_instance, this);
    }
}