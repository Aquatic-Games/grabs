using System;
using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

public sealed class D3D11Buffer : Buffer
{
    public ID3D11Buffer Buffer;

    public unsafe D3D11Buffer(ID3D11Device device, BufferDescription description, void* data) : base(description)
    {
        BindFlags flags = description.Type switch
        {
            BufferType.Vertex => BindFlags.VertexBuffer,
            BufferType.Index => BindFlags.IndexBuffer,
            BufferType.Constant => BindFlags.ConstantBuffer,
            _ => throw new ArgumentOutOfRangeException()
        };

        Vortice.Direct3D11.BufferDescription desc = new Vortice.Direct3D11.BufferDescription()
        {
            BindFlags = flags,
            ByteWidth = (int) description.SizeInBytes,
            Usage = description.Dynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
        };

        SubresourceData sData = new SubresourceData(data);

        Buffer = device.CreateBuffer(desc, sData);
    }
    
    public override void Dispose()
    {
        Buffer.Dispose();
    }
}