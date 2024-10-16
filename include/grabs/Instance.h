#pragma once

#include <memory>
#include <cstdint>
#include <vector>
#include <functional>

#include "Common.h"
#include "Device.h"
#include "Surface.h"

namespace grabs {

    struct InstanceInfo {
        bool Debug;
        std::function<std::vector<const char*>()> GetInstanceExtensions;
        std::function<void*(void* instance)> CreateSurface;
    };

    class Instance {
    public:
        virtual ~Instance() = default;

        virtual std::unique_ptr<Device> CreateDevice(Surface* surface, uint32_t adapterIndex = 0);

        virtual std::vector<Adapter> EnumerateAdapters() = 0;

        static std::unique_ptr<Instance> Create(const InstanceInfo& info = {});
    };

}
