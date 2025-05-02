using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D11_BIND_FLAG;
using static TerraFX.Interop.DirectX.D3D11_CPU_ACCESS_FLAG;
using static TerraFX.Interop.DirectX.D3D11_USAGE;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Buffer : Buffer
{
    public override bool IsDisposed { get; protected set; }

    public readonly ID3D11Buffer* Buffer;

    public D3D11Buffer(ID3D11Device* device, ref readonly BufferInfo info, void* pData)
    {
        D3D11_BIND_FLAG bindFlags = 0;
        D3D11_USAGE usage = D3D11_USAGE_DEFAULT;
        D3D11_CPU_ACCESS_FLAG cpuFlags = 0;

        if (info.Usage.HasFlag(BufferUsage.Vertex))
            bindFlags |= D3D11_BIND_VERTEX_BUFFER;
        if (info.Usage.HasFlag(BufferUsage.Index))
            bindFlags |= D3D11_BIND_INDEX_BUFFER;
        if (info.Usage.HasFlag(BufferUsage.Constant))
            bindFlags |= D3D11_BIND_CONSTANT_BUFFER;

        if (info.Usage.HasFlag(BufferUsage.CopySrc))
        {
            usage = D3D11_USAGE_STAGING;
            cpuFlags |= D3D11_CPU_ACCESS_WRITE;
        }

        D3D11_BUFFER_DESC bufferDesc = new()
        {
            BindFlags = (uint) bindFlags,
            ByteWidth = info.Size,
            Usage = usage,
            CPUAccessFlags = (uint) cpuFlags,
        };

        D3D11_SUBRESOURCE_DATA resourceData = new()
        {
            pSysMem = pData
        };

        fixed (ID3D11Buffer** buffer = &Buffer)
            device->CreateBuffer(&bufferDesc, &resourceData, buffer).Check("Create buffer");
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;

        Buffer->Release();
    }
}