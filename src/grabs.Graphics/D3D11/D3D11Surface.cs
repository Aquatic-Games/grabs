using System.Diagnostics;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Surface : Surface
{
    public readonly nint Hwnd;

    public D3D11Surface(ref readonly SurfaceInfo info)
    {
        Debug.Assert(info.Type == SurfaceType.Windows, "info.Type == SurfaceType.Windows");

        Hwnd = info.Window.Windows;
    }
    
    public override void Dispose() { }
}