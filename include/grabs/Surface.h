#pragma once

#include <functional>
#include <memory>

namespace grabs {

    class Instance;

    class Surface {
    public:
        virtual ~Surface() = default;

        static std::unique_ptr<Surface> Vulkan(Instance* instance, const std::function<void*(void* instance)>& createSurfaceFn);
    };

}
