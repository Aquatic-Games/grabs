namespace grabs.Graphics.OpenGL;

public class OpenGLBackend : IBackend
{
    public static string Name => "OpenGL";

    public OpenGLBackend()
    {
        nint display = Egl.GetDisplay(0);
        Console.WriteLine(display);
    }
    
    public Instance CreateInstance(ref readonly InstanceInfo info)
    {
        throw new NotImplementedException();
    }
}