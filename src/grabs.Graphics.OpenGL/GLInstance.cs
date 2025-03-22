using System.Runtime.InteropServices;
using Silk.NET.OpenGL;

namespace grabs.Graphics.OpenGL;

internal sealed unsafe class GLInstance : Instance
{
    public readonly GL Gl;
    
    public override string Backend => OpenGLBackend.Name;

    public GLInstance(ref readonly InstanceInfo info, OpenGLBackend backend)
    {
        Gl = GL.GetApi(backend.GetProcAddressFunc);
    }
    
    public override Adapter[] EnumerateAdapters()
    {
        Adapter adapter = new Adapter(0, 0, Gl.GetStringS(StringName.Renderer), AdapterType.Dedicated, 0,
            new AdapterFeatures(), new AdapterLimits());
        
        return [adapter];
    }
    
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }
    
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        return new GLSurface();
    }
    
    public override void Dispose()
    {
        Gl.Dispose();
    }
}