namespace grabs.Graphics;

public struct Viewport
{
    public int X;
    public int Y;
    public uint Width;
    public uint Height;
    public float MinDepth;
    public float MaxDepth;

    public Viewport(int x, int y, uint width, uint height, float minDepth = 0, float maxDepth = 1)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
        MinDepth = minDepth;
        MaxDepth = maxDepth;
    }
}