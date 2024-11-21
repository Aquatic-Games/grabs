#pragma once

#include "grabs/Surface.h"

#include <d3d11.h>

namespace grabs::D3D11
{
    class DXGISurface : public Surface
    {
    public:
        HWND Window{};

        explicit DXGISurface(const HWND window)
        {
            Window = window;
        }
    };
}