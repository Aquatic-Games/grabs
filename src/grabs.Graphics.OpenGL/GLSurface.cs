namespace grabs.Graphics.OpenGL;

internal sealed unsafe class GLSurface : Surface
{
    public GLSurface()
    {
        
    }
    
    public override Format[] EnumerateSupportedFormats(in Adapter adapter)
    {
        return [Format.B8G8R8A8_UNorm, Format.B8G8R8A8_UNorm_SRGB, Format.R8G8B8A8_UNorm, Format.R8G8B8A8_UNorm_SRGB];
    }
    
    public override void Dispose()
    {
        
    }
}