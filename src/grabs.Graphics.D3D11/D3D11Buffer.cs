using Vortice.Direct3D11;

namespace grabs.Graphics.D3D11;

internal sealed unsafe class D3D11Buffer : Buffer
{
    private readonly ID3D11DeviceContext _context;
    
    public readonly ID3D11Buffer Buffer;

    public readonly bool IsDynamic;

    public readonly MapMode MapMode;

    public D3D11Buffer(ID3D11Device device, ID3D11DeviceContext context, ref readonly BufferInfo info, void* pData) 
        : base(in info)
    {
        _context = context;

        BindFlags flags = BindFlags.None;
        ResourceUsage usage = ResourceUsage.Default;
        CpuAccessFlags cpuFlags = CpuAccessFlags.None;
        ResourceOptionFlags miscFlags = ResourceOptionFlags.None;

        if (info.Usage.HasFlag(BufferUsage.Vertex))
            flags |= BindFlags.VertexBuffer;
        if (info.Usage.HasFlag(BufferUsage.Index))
            flags |= BindFlags.IndexBuffer;
        if (info.Usage.HasFlag(BufferUsage.Constant))
            flags |= BindFlags.ConstantBuffer;

        if (info.Usage.HasFlag(BufferUsage.TransferSrc))
            usage = ResourceUsage.Staging;

        if (info.Usage.HasFlag(BufferUsage.MapWrite))
        {
            usage = ResourceUsage.Dynamic;
            cpuFlags |= CpuAccessFlags.Write;
            IsDynamic = true;
            MapMode = MapMode.WriteDiscard;
        }

        BufferDescription description = new BufferDescription()
        {
            BindFlags = flags,
            ByteWidth = info.Size,
            Usage = usage,
            CPUAccessFlags = cpuFlags,
            MiscFlags = miscFlags
        };

        Buffer = device.CreateBuffer(in description, (nint) pData);
    }
    
    protected override MappedData Map()
    {
        MappedSubresource resource = _context.Map(Buffer, MapMode);
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