#pragma once

namespace grabs {

    class Surface {

    };

    class VulkanSurface : public Surface {
    public:
        void* VkSurface;
    };

}
