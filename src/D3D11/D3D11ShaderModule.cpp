#include "D3D11ShaderModule.h"

#include <spirv_hlsl.hpp>
#include <spirv_cross.hpp>
#include <spirv.hpp>
#include <d3dcompiler.h>
#include <stdexcept>
#include <format>
#include <string>
#include <iostream>

namespace grabs::D3D11
{
    D3D11ShaderModule::D3D11ShaderModule(const ShaderModuleDescription& description)
    {
        auto compiler = spirv_cross::CompilerHLSL(reinterpret_cast<const uint32_t*>(description.Spirv.data()),
                                                  description.Spirv.size() / 4);

        spirv_cross::CompilerHLSL::Options options
        {
            .shader_model = 50
        };
        compiler.set_hlsl_options(options);

        spv::ExecutionModel model = spv::ExecutionModelMax;
        const char* target = nullptr;
        switch (description.Stage)
        {
        case ShaderStage::Vertex:
            model = spv::ExecutionModelVertex;
            target = "vs_5_0";
            break;
        case ShaderStage::Pixel:
            model = spv::ExecutionModelFragment;
            target = "ps_5_0";
            break;
        case ShaderStage::Geometry:
            model = spv::ExecutionModelGeometry;
            target = "gs_5_0";
            break;
        case ShaderStage::Compute:
            // GL compute? Hmm.
            model = spv::ExecutionModelGLCompute;
            target = "cs_5_0";
            break;
        }

        compiler.set_entry_point(description.EntryPoint, model);

        std::string hlsl = compiler.compile();
        std::cout << hlsl << std::endl;

        ID3DBlob* shader;
        ID3DBlob* error;
        if (FAILED(D3DCompile(hlsl.c_str(), hlsl.size(), nullptr, nullptr, nullptr, "main", target, 0, 0, &shader, &error)))
        {
            throw std::runtime_error(std::format("Failed to compile shader: {}", shader->GetBufferPointer()));
        }

        if (error)
            error->Release();

        Blob = shader;
    }

    D3D11ShaderModule::~D3D11ShaderModule()
    {
        Blob->Release();
    }
}
