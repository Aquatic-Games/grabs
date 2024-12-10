using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.DirectX;
using static grabs.Graphics.D3D11.D3D11Result;
using static TerraFX.Interop.DirectX.D3D11_BIND_FLAG;
using static TerraFX.Interop.DirectX.D3D11_USAGE;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11Buffer : Buffer
{
    public readonly ID3D11Buffer* Buffer;

    public D3D11Buffer(ID3D11Device* device, in BufferDescription description, void* data)
    {
        D3D11_BIND_FLAG bindFlags = description.Type switch
        {
            BufferType.Vertex => D3D11_BIND_VERTEX_BUFFER,
            BufferType.Index => D3D11_BIND_INDEX_BUFFER,
            BufferType.Constant => D3D11_BIND_CONSTANT_BUFFER,
            _ => throw new ArgumentOutOfRangeException()
        };

        D3D11_BUFFER_DESC desc = new()
        {
            BindFlags = (uint) bindFlags,
            ByteWidth = description.Size,
            Usage = description.Dynamic ? D3D11_USAGE_DYNAMIC : D3D11_USAGE_DEFAULT
        };

        D3D11_SUBRESOURCE_DATA resData = new()
        {
            pSysMem = data
        };

        fixed (ID3D11Buffer** buffer = &Buffer)
            CheckResult(device->CreateBuffer(&desc, data == null ? null : &resData, buffer), "Create buffer");
    }
    
    public override void Dispose()
    {
        Buffer->Release();
    }
}