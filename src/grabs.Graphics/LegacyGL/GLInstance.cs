using Silk.NET.OpenGL;

namespace grabs.Graphics.LegacyGL;

internal sealed class GLInstance : Instance
{
    private readonly GLBase _gl;
    
    public override bool IsDisposed { get; protected set; }

    public override Backend Backend => Backend.LegacyGL;

    public GLInstance(Func<string, nint> getProcAddress)
    {
        GL gl = GL.GetApi(getProcAddress);
        
        int majorVersion = gl.GetInteger(GetPName.MajorVersion);

        if (majorVersion >= 4)
            _gl = new GL43(gl);
        else
            throw new NotImplementedException();
    }
    
    public override Adapter[] EnumerateAdapters()
    {
        string name = _gl.GetString(StringName.Renderer);

        return [new Adapter(0, name, 0, AdapterType.Dedicated)];
    }

    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }

    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
    }
}