using Silk.NET.OpenGL;

namespace grabs.Graphics.LegacyGL;

internal abstract class GLBase
{
    public abstract string GetString(StringName name);
    
    public abstract void ClearColor(float r, float g, float b, float a);

    public abstract void Clear(ClearBufferMask mask);
}