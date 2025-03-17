using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

internal sealed unsafe class D3D11Buffer : Buffer
{
    private readonly ID3D11DeviceContext _context;
    
    public readonly ID3D11Buffer Buffer;

    public readonly bool IsDynamic;

    public D3D11Buffer(ID3D11Device device, ID3D11DeviceContext context, ref readonly BufferInfo info, void* pData) 
        : base(in info)
    {
        _context = context;

        //IsDynamic = info.Usage == BufferUsage.Dynamic;
        
        BindFlags flags = info.Usage switch
        {
            BufferUsage.Vertex => BindFlags.VertexBuffer,
            BufferUsage.Index => BindFlags.IndexBuffer,
            BufferUsage.Constant => BindFlags.ConstantBuffer,
            _ => throw new ArgumentOutOfRangeException()
        };

        BufferDescription description = new BufferDescription()
        {
            BindFlags = flags,
            ByteWidth = info.Size,
            Usage = IsDynamic ? ResourceUsage.Dynamic : ResourceUsage.Default,
            CPUAccessFlags = IsDynamic ? CpuAccessFlags.Write : CpuAccessFlags.None
        };

        Buffer = device.CreateBuffer(in description, (nint) pData);
    }
    
    protected override MappedData Map(MapMode mode)
    {
        MappedSubresource resource = _context.Map(Buffer, mode.ToD3D());
        return new MappedData(resource.DataPointer);
    }
    
    protected override void Unmap()
    {
        _context.Unmap(Buffer);
    }
    
    public override void Dispose()
    {
        Buffer.Dispose();
    }
}