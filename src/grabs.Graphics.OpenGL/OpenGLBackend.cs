namespace grabs.Graphics.OpenGL;

public class OpenGLBackend : IBackend
{
    public static string Name => "OpenGL";
    
    public Instance CreateInstance(ref readonly InstanceInfo info)
    {
        return new GLInstance(in info);
    }
}