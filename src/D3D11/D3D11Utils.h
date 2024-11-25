#pragma once

#include <d3d11.h>
#include <stdexcept>
#include <format>

#include "grabs/Common.h"

#define D3D11_CHECK_RESULT(Result) \
{ \
    auto res = Result; \
    if (FAILED(res)) \
        throw std::runtime_error(std::format("{}:{}: {} failed with error code {}", __FILE__, __LINE__, #Result, res)); \
}

namespace grabs::D3D11::Utils
{
    DXGI_FORMAT FormatToD3D(Format format);
}
