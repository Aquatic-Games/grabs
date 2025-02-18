using System.Diagnostics;
using grabs.Core;
using Silk.NET.Vulkan;
using Silk.NET.Vulkan.Extensions.KHR;

namespace grabs.Graphics.Vulkan;

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
        Debug.Assert(info.Type != SurfaceType.Windows || (info.Display.Windows != 0 && info.Window.Windows != 0),
            $"SurfaceType is Windows however Display (0x{info.Display.Windows:X}) and/or Window (0x{info.Window.Windows:X}) pointer(s) are null.");
        Debug.Assert(info.Type != SurfaceType.Xlib || (info.Display.Xlib != 0 && info.Window.Xlib != 0),
            $"SurfaceType is Xlib however Display (0x{info.Display.Xlib:X}) and/or Window (0x{info.Window.Xlib:X}) pointer(s) are null.");
        Debug.Assert(info.Type != SurfaceType.Xcb || (info.Display.Xcb != 0 && info.Window.Xcb != 0),
            $"SurfaceType is Xcb however Display (0x{info.Display.Xcb:X}) and/or Window (0x{info.Window.Xcb:X}) pointer(s) are null.");
        Debug.Assert(info.Type != SurfaceType.Wayland || (info.Display.Wayland != 0 && info.Window.Wayland != 0),
            $"SurfaceType is Wayland however Display (0x{info.Display.Wayland:X}) and/or Window (0x{info.Window.Wayland:X}) pointer(s) are null.");
        
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

                GrabsLog.Log("Creating Win32 surface.");
                _win32Surface!.CreateWin32Surface(instance, &surfaceInfo, null, out Surface)
                    .Check("Create Win32 surface");
                
                break;
            }
            case SurfaceType.Xlib:
            {
                XlibSurfaceCreateInfoKHR surfaceInfo = new XlibSurfaceCreateInfoKHR()
                {
                    SType = StructureType.XlibSurfaceCreateInfoKhr,
                    Dpy = (IntPtr*) info.Display.Xlib,
                    Window = info.Window.Xlib
                };

                if (!vk.TryGetInstanceExtension(instance, out _xlibSurface))
                    throw new Exception("Failed to get Xlib extension.");

                GrabsLog.Log("Creating Xlib surface.");
                _xlibSurface!.CreateXlibSurface(instance, &surfaceInfo, null, out Surface)
                    .Check("Create Xlib surface");
                
                break;
            }
            case SurfaceType.Xcb:
            {
                XcbSurfaceCreateInfoKHR xcbSurface = new XcbSurfaceCreateInfoKHR()
                {
                    SType = StructureType.XcbSurfaceCreateInfoKhr,
                    Connection = (IntPtr*) info.Display.Xcb,
                    Window = info.Window.Xcb
                };

                if (!vk.TryGetInstanceExtension(instance, out _xcbSurface))
                    throw new Exception("Failed to get XCB extension.");

                GrabsLog.Log("Creating XCB surface.");
                _xcbSurface!.CreateXcbSurface(instance, &xcbSurface, null, out Surface)
                    .Check("Create XCB surface");
                
                break;
            }
            case SurfaceType.Wayland:
            {
                // Silk.NET "bug", types are IntPtr* but you must cast the existing pointer to this type, NOT take the
                // address of it.
                WaylandSurfaceCreateInfoKHR waylandSurface = new WaylandSurfaceCreateInfoKHR()
                {
                    SType = StructureType.WaylandSurfaceCreateInfoKhr,
                    Display = (IntPtr*) info.Display.Wayland,
                    Surface = (IntPtr*) info.Window.Wayland
                };

                if (!vk.TryGetInstanceExtension(instance, out _waylandSurface))
                    throw new Exception("Failed to get Wayland extension.");

                GrabsLog.Log("Creating Wayland surface.");
                _waylandSurface!.CreateWaylandSurface(instance, &waylandSurface, null, out Surface)
                    .Check("Create Wayland surface");
                
                break;
            }
            default:
                throw new ArgumentOutOfRangeException();
        }
    }

    public override Format[] EnumerateSupportedFormats(in Adapter adapter)
    {
        Debug.Assert(adapter.Handle != IntPtr.Zero);
        PhysicalDevice device = new PhysicalDevice(adapter.Handle);

        uint numFormats;
        _khrSurface.GetPhysicalDeviceSurfaceFormats(device, Surface, &numFormats, null);
        SurfaceFormatKHR* vkFormats = stackalloc SurfaceFormatKHR[(int) numFormats];
        _khrSurface.GetPhysicalDeviceSurfaceFormats(device, Surface, &numFormats, vkFormats);

        List<Format> formats = [];
        for (int i = 0; i < numFormats; i++)
        {
            if (vkFormats[i].ColorSpace == ColorSpaceKHR.SpaceSrgbNonlinearKhr)
            {
                Format format = vkFormats[i].Format.ToGrabs();
                if (format == Format.Unknown)
                    continue;
                
                formats.Add(format);
            }
        }

        return formats.ToArray();
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