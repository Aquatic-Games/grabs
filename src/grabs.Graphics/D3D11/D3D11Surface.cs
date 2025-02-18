using System.Diagnostics;

namespace grabs.Graphics.D3D11;

internal sealed class D3D11Surface : Surface
{
    public readonly nint Hwnd;

    public D3D11Surface(ref readonly SurfaceInfo info)
    {
        Debug.Assert(info.Type == SurfaceType.Windows);

        Hwnd = info.Window.Windows;
    }

    public override Format[] EnumerateSupportedFormats(in Adapter adapter)
        => [Format.B8G8R8A8_UNorm, Format.B8G8R8A8_UNorm_SRGB, Format.R8G8B8A8_UNorm, Format.R8G8B8A8_UNorm_SRGB];
    
    public override void Dispose() { }
}