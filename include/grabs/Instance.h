#pragma once

#include <memory>
#include <string>
#include <vector>

#include "Common.h"
#include "Surface.h"

namespace grabs
{
    struct InstanceInfo
    {
        bool Debug;
        Backend BackendHint;
        std::string AppName;
        GrabsDebugCallback DebugCallback;
        void* DebugCallbackData;
    };

    class Instance
    {
    public:
        virtual ~Instance() = default;

        virtual std::vector<Adapter> EnumerateAdapters() = 0;

        virtual std::unique_ptr<Surface> CreateSurface(const SurfaceDescription& description) = 0;

        static std::unique_ptr<Instance> Create(const InstanceInfo& info);
    };
}
