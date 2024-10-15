#pragma once

#include <memory>

namespace grabs {

    struct InstanceInfo {
        bool Debug;
        const char** (*GetInstanceExtensions)();
        void* (*CreateSurface)();

        InstanceInfo() {
            Debug = false;
            CreateSurface = nullptr;
            GetInstanceExtensions = nullptr;
        }
    };

    class Instance {
    public:
        static std::unique_ptr<Instance> Create(const InstanceInfo& info = {});
    };

}
