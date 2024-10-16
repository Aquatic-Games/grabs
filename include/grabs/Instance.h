#pragma once

#include <memory>
#include <cstdint>
#include <vector>
#include <functional>

#include "Common.h"

namespace grabs {

    struct InstanceInfo {
        bool Debug;
        std::function<std::vector<const char*>()> GetInstanceExtensions;
        std::function<void*(void* instance)> CreateSurface;
    };

    class Instance {
    public:
        virtual ~Instance() = default;

        virtual std::vector<Adapter> EnumerateAdapters() = 0;

        static std::unique_ptr<Instance> Create(const InstanceInfo& info = {});
    };

}
