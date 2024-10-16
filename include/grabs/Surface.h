#pragma once

#include <functional>

namespace grabs {

    class Surface {
    public:
        virtual ~Surface() = default;
    };

    class VulkanSurface final : public Surface {
    public:
        explicit VulkanSurface(const std::function<void*(void* instance)>& createSurfaceFn) {
            CreateSurfaceFn = createSurfaceFn;
        }

        std::function<void*(void* instance)> CreateSurfaceFn;
    };

}
