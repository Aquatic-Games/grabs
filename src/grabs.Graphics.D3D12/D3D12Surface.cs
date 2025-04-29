namespace grabs.Graphics.D3D12;

internal sealed class D3D12Surface : Surface
{
    public override bool IsDisposed { get; protected set; }

    public readonly nint Hwnd;

    public D3D12Surface(ref readonly SurfaceInfo info)
    {
        Hwnd = info.Window;
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
    }
}