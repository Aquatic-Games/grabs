using System;
using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public sealed class GL43Instance : Instance
{
    public readonly GL GL;

    public override GraphicsApi Api => GraphicsApi.OpenGL;

    public GL43Instance(Func<string, nint> getProcAddressFunc)
    {
        GL = GL.GetApi(getProcAddressFunc);
    }
    
    public override Device CreateDevice(Adapter? adapter = null)
    {
        return new GL43Device(GL);
    }

    public override Adapter[] EnumerateAdapters()
    {
        // OpenGL doesn't support enumerating adapters - but we still need to implement the functionality.
        // So we simply get the GL renderer info and return it.
        // Sure there is no dedicated memory but who needs VRAM anyway :)
        
        string renderer = GL.GetStringS(StringName.Renderer);

        ulong memory = 0;

        if (GL.IsExtensionPresent("GL_NVX_gpu_memory_info"))
            memory = (ulong) GL.GetInteger((GLEnum) 0x9047) * 1024;
        
        Adapter adapter = new Adapter(0, renderer, memory, AdapterType.Discrete);

        return [adapter];
    }

    public override void Dispose()
    {
        GL.Dispose();
    }
}