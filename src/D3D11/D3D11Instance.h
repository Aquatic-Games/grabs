#pragma once

#include "grabs/Instance.h"

#include <d3d11.h>

namespace grabs::D3D11
{
    class D3D11Instance : public Instance
    {
    public:
        bool Debug{};
        IDXGIFactory1* Factory{};

        explicit D3D11Instance(const InstanceInfo& info);
        ~D3D11Instance() override;

        [[nodiscard]] grabs::Backend Backend() const override;

        std::unique_ptr<Device> CreateDevice(Surface* surface, uint32_t adapterIndex) override;

        std::vector<Adapter> EnumerateAdapters() override;
    };
}
