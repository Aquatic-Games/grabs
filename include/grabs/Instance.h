#pragma once

#include <memory>
#include <string>
#include <vector>

#include "Common.h"

namespace grabs
{
    struct InstanceInfo
    {
        bool Debug;
        Backend BackendHint;
        std::string AppName;
    };

    class Instance
    {
    public:
        virtual ~Instance() = default;

        virtual std::vector<Adapter> EnumerateAdapters() = 0;

        static std::unique_ptr<Instance> Create(const InstanceInfo& info);
    };
}
