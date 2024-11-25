#pragma once

#include <d3d11.h>

#include "grabs/Device.h"
#include "D3D11CommandList.h"
#include "D3D11Buffer.h"

namespace grabs::D3D11
{
    class D3D11Device final : public Device
    {
    public:
        IDXGIFactory1* Factory{};
        ID3D11Device* Device{};
        ID3D11DeviceContext* Context{};

        D3D11Device(IDXGIFactory1* factory, IDXGIAdapter1* adapter, bool debug);
        ~D3D11Device() override;

        std::unique_ptr<Swapchain> CreateSwapchain(const SwapchainDescription& description, Surface* surface) override;
        std::unique_ptr<CommandList> CreateCommandList() override;
        std::unique_ptr<Buffer> CreateBuffer(const BufferDescription& description, void* data) override;
        std::unique_ptr<ShaderModule> CreateShaderModule(const ShaderModuleDescription& description) override;
        std::unique_ptr<Pipeline> CreatePipeline(const PipelineDescription& description) override;

        void SubmitCommandList(CommandList* list) override;
    };
}