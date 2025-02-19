namespace grabs.Graphics;

public struct VertexBufferInfo
{
    public uint Binding;

    public uint Stride;

    public VertexBufferInfo(uint binding, uint stride)
    {
        Binding = binding;
        Stride = stride;
    }
}