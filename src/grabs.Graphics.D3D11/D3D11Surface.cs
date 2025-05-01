namespace grabs.Graphics.D3D11;

internal sealed class D3D11Surface(ref readonly SurfaceInfo info) : Surface
{
    public override bool IsDisposed { get; protected set; }

    public readonly nint Hwnd = info.Window;

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
    }
}