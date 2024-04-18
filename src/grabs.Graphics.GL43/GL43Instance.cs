using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public sealed class GL43Instance : Instance
{
    public readonly GL GL;

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
        Adapter adapter = new Adapter(0, renderer, 0, AdapterType.Discrete);

        return [adapter];
    }

    public override void Dispose()
    {
        GL.Dispose();
    }
}