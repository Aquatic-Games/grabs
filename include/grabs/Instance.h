#pragma once

#include <memory>
#include <cstdint>

namespace grabs {

    struct InstanceInfo {
        bool Debug;
        const char** (*GetInstanceExtensions)(uint32_t* numExtensions);
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
