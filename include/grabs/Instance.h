#pragma once

#include <memory>
#include <cstdint>
#include <vector>
#include <functional>

#include "Common.h"
#include "Device.h"
#include "Surface.h"

namespace grabs
{
    struct InstanceInfo
    {
        bool Debug;
        Backend BackendHint;
        std::function<std::vector<const char*>()> GetInstanceExtensions;
    };

    class Instance
    {
    public:
        virtual ~Instance() = default;

        [[nodiscard]] virtual Backend Backend() const = 0;

        virtual std::unique_ptr<Device> CreateDevice(Surface* surface, const Adapter& adapter) = 0;

        std::unique_ptr<Device> CreateDevice(Surface* surface)
        {
            const auto adapters = EnumerateAdapters();
            return CreateDevice(surface, adapters[0]);
        }

        virtual std::vector<Adapter> EnumerateAdapters() = 0;

        static std::unique_ptr<Instance> Create(const InstanceInfo& info = {});
    };
}
