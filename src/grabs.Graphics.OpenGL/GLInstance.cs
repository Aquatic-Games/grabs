using Silk.NET.OpenGL;

namespace grabs.Graphics.OpenGL;

internal sealed unsafe class GLInstance : Instance
{
    private readonly void* _context;
    
    public readonly GL Gl;
    
    public override string Backend => OpenGLBackend.Name;

    public GLInstance(ref readonly InstanceInfo info)
    {
        void* display = Egl.GetDisplay(Egl.DefaultDisplay);
        
        int major, minor;
        Egl.Initialize(display, &major, &minor);

        if (major < 1 || minor < 5)
            throw new Exception($"EGL version not supported: {major}.{minor}");

        int* attribs = stackalloc int[]
        {
            Egl.RedSize, 1,
            Egl.GreenSize, 1,
            Egl.BlueSize, 1,
            Egl.AlphaSize, 0,
            Egl.DepthSize, 0,
            Egl.StencilSize, 0
        };

        void* config;
        Egl.ChooseConfig(display, attribs, &config, 1, null);
        
        
    }
    
    public override Adapter[] EnumerateAdapters()
    {
        throw new NotImplementedException();
    }
    
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }
    
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        throw new NotImplementedException();
    }
}