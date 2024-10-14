#pragma once

#include <memory>

namespace grabs {

    class Instance {
    public:
        static std::unique_ptr<Instance> Create();
    };

}
