#pragma once

#include <memory>
#include <cstdint>
#include <vector>
#include <functional>

namespace grabs {

    struct InstanceInfo {
        bool Debug;
        std::function<std::vector<const char*>()> GetInstanceExtensions;
        std::function<void*(void* instance)> CreateSurface;
    };

    class Instance {
    public:
        static std::unique_ptr<Instance> Create(const InstanceInfo& info = {});
    };

}
