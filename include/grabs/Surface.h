#pragma once

#include <functional>
#include <memory>

namespace grabs
{
    class Instance;

    class Surface
    {
    public:
        virtual ~Surface() = default;

        // TODO: I don't like this way of handling surfaces. Maybe move to some kind of interface/callback system instead?
        static std::unique_ptr<Surface> Vulkan(Instance* instance, const std::function<void*(void* instance)>& createSurfaceFn);
        static std::unique_ptr<Surface> DXGI(size_t hwnd);
    };
}
