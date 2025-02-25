using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

internal sealed unsafe class D3D11Buffer : Buffer
{
    public readonly ID3D11Buffer Buffer;

    public D3D11Buffer(ID3D11Device device, ref readonly BufferInfo info, void* pData)
    {
        BindFlags flags = info.Type switch
        {
            BufferType.Vertex => BindFlags.VertexBuffer,
            BufferType.Index => BindFlags.IndexBuffer,
            _ => throw new ArgumentOutOfRangeException()
        };

        BufferDescription description = new BufferDescription()
        {
            BindFlags = flags,
            ByteWidth = info.Size,
            Usage = ResourceUsage.Default,
        };

        Buffer = device.CreateBuffer(in description, (nint) pData);
    }
    
    protected override MappedData Map(MapType type)
    {
        throw new NotImplementedException();
    }
    
    protected override void Unmap()
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        Buffer.Dispose();
    }
}