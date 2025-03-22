namespace grabs.Graphics.OpenGL;

public class OpenGLBackend : IBackend
{
    public static string Name => "OpenGL";

    public Func<string, nint> GetProcAddressFunc;

    public Action<int> PresentFunc;

    public OpenGLBackend(Func<string, nint> getProcAddressFunc, Action<int> presentFunc)
    {
        GetProcAddressFunc = getProcAddressFunc;
        PresentFunc = presentFunc;
    }

    public Instance CreateInstance(ref readonly InstanceInfo info)
    {
        return new GLInstance(in info, this);
    }
}