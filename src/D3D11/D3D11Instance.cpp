#include "D3D11Instance.h"
#include "D3D11Utils.h"

namespace grabs::D3D11
{
    D3D11Instance::D3D11Instance(const InstanceInfo& info)
    {
        D3D11_CHECK_RESULT(CreateDXGIFactory1(__uuidof(IDXGIFactory1), reinterpret_cast<void**>(&Factory)));
    }

    D3D11Instance::~D3D11Instance()
    {
        Factory->Release();
    }

    std::unique_ptr<Device> D3D11Instance::CreateDevice(Surface* surface, uint32_t adapterIndex)
    {
        return nullptr;
    }

    std::vector<Adapter> D3D11Instance::EnumerateAdapters()
    {
        std::vector<Adapter> adapters;

        IDXGIAdapter1* dxgiAdapter;
        for (UINT i = 0; Factory->EnumAdapters1(i, &dxgiAdapter) != DXGI_ERROR_NOT_FOUND; i++)
        {
            DXGI_ADAPTER_DESC1 desc;
            D3D11_CHECK_RESULT(dxgiAdapter->GetDesc1(&desc));

            std::wstring wName = desc.Description;
            const std::string name(wName.begin(), wName.end());

            Adapter adapter
            {
                .Index = i,
                .Name = name,
                .Memory = desc.DedicatedVideoMemory,
                .Supports = AdapterSupports
                {
                    .GeometryShader = true,
                    .Anisotropy = true,
                    .MaxAnisotropy = D3D11_MAX_MAXANISOTROPY
                }
            };

            adapters.push_back(adapter);
        }

        return adapters;
    }
}
