#include "D3D11CommandList.h"
#include "D3D11Utils.h"

namespace grabs::D3D11
{
    D3D11CommandList::D3D11CommandList(ID3D11Device* device)
    {
        D3D11_CHECK_RESULT(device->CreateDeferredContext(0, &Context));
    }

    D3D11CommandList::~D3D11CommandList()
    {
        if (CommandList)
            CommandList->Release();

        Context->Release();
    }
}
