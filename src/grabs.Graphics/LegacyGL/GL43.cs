using Silk.NET.OpenGL;

namespace grabs.Graphics.LegacyGL;

internal sealed class GL43 : GLBase
{
    private readonly GL _gl;

    public GL43(GL gl)
    {
        _gl = gl;
    }

    public override string GetString(StringName name)
    {
        return _gl.GetStringS(name);
    }

    public override void ClearColor(float r, float g, float b, float a)
    {
        _gl.ClearColor(r, g, b, a);
    }

    public override void Clear(ClearBufferMask mask)
    {
        _gl.Clear(mask);
    }
}