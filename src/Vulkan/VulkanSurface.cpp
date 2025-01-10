#include "VulkanSurface.h"

#include <stdexcept>

#ifdef GS_OS_LINUX
#include <X11/Xlib-xcb.h>
#include <vulkan/vulkan_xlib.h>
#include <vulkan/vulkan_xcb.h>
#include <vulkan/vulkan_wayland.h>
#endif

#include "VulkanUtils.h"

namespace grabs::Vulkan
{
    VulkanSurface::VulkanSurface(VkInstance instance, const SurfaceDescription& description)
    {
        Instance = instance;

        switch (description.Type)
        {
#ifdef GS_OS_WINDOWS
            case SurfaceType::Windows:
                break;
#endif
#ifdef GS_OS_LINUX
            case SurfaceType::Xlib:
            {
                VkXlibSurfaceCreateInfoKHR xlibInfo
                {
                    .sType = VK_STRUCTURE_TYPE_XLIB_SURFACE_CREATE_INFO_KHR,
                    .dpy = description.Display.Xlib,
                    .window = description.Window.Xlib
                };

                VK_CHECK_RESULT(vkCreateXlibSurfaceKHR(instance, &xlibInfo, nullptr, &Surface));
                break;
            }

            case SurfaceType::XCB:
            {
                VkXcbSurfaceCreateInfoKHR xcbInfo
                {
                    .sType = VK_STRUCTURE_TYPE_XCB_SURFACE_CREATE_INFO_KHR,
                    .connection = description.Display.XCB,
                    .window = description.Window.XCB
                };

                VK_CHECK_RESULT(vkCreateXcbSurfaceKHR(instance, &xcbInfo, nullptr, &Surface));
                break;
            }

            case SurfaceType::Wayland:
                {
                    VkWaylandSurfaceCreateInfoKHR waylandInfo
                    {
                        .sType = VK_STRUCTURE_TYPE_WAYLAND_SURFACE_CREATE_INFO_KHR,
                        .display = reinterpret_cast<struct ::wl_display*>(description.Display.Wayland),
                        .surface = reinterpret_cast<struct ::wl_surface*>(description.Window.Wayland)
                    };

                    VK_CHECK_RESULT(vkCreateWaylandSurfaceKHR(instance, &waylandInfo, nullptr, &Surface));
                    break;
                }
#endif

            default:
                throw std::runtime_error("Unsupported surface type.");
        }
    }

    VulkanSurface::~VulkanSurface()
    {
        vkDestroySurfaceKHR(Instance, Surface, nullptr);
    }
}
