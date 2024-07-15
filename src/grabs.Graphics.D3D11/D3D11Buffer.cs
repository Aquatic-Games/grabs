using System;
using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.D3D11_BIND_FLAG;
using static TerraFX.Interop.DirectX.D3D11_CPU_ACCESS_FLAG;
using static TerraFX.Interop.DirectX.D3D11_USAGE;
using static grabs.Graphics.D3D11.D3DResult;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed unsafe class D3D11Buffer : Buffer
{
    public ID3D11Buffer* Buffer;

    public D3D11Buffer(ID3D11Device* device, BufferDescription description, void* pData) : base(description)
    {
        D3D11_BIND_FLAG flags = description.Type switch
        {
            BufferType.Vertex => D3D11_BIND_VERTEX_BUFFER,
            BufferType.Index => D3D11_BIND_INDEX_BUFFER,
            BufferType.Constant => D3D11_BIND_CONSTANT_BUFFER,
            _ => throw new ArgumentOutOfRangeException()
        };

        D3D11_BUFFER_DESC desc = new D3D11_BUFFER_DESC()
        {
            BindFlags = (uint) flags,
            ByteWidth = description.SizeInBytes,
            Usage = description.Dynamic ? D3D11_USAGE_DYNAMIC : D3D11_USAGE_DEFAULT,
            CPUAccessFlags = description.Dynamic ? (uint) D3D11_CPU_ACCESS_WRITE : 0
        };

        D3D11_SUBRESOURCE_DATA sData = new D3D11_SUBRESOURCE_DATA()
        {
            pSysMem = pData
        };
        
        fixed (ID3D11Buffer** buffer = &Buffer)
            CheckResult(device->CreateBuffer(&desc, pData == null ? null : &sData, buffer), "Create buffer");
    }
    
    public override void Dispose()
    {
        Buffer->Release();
    }
}