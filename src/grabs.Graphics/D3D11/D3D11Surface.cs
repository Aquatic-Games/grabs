namespace grabs.Graphics.D3D11;

public sealed class D3D11Surface : Surface
{
    public readonly nint Hwnd;

    public D3D11Surface(nint hwnd)
    {
        Hwnd = hwnd;
    }

    public override void Dispose()
    {
        // Unused here.
    }
}