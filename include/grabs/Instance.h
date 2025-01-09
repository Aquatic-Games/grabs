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
        std::vector<Adapter> EnumerateAdapters();

        static std::unique_ptr<Instance> Create();
    };
}
