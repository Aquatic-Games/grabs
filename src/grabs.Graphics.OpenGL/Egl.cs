using System.Runtime.InteropServices;

namespace grabs.Graphics.OpenGL;

internal static class Egl
{
    [DllImport("EGL", EntryPoint = "eglGetDisplay")]
    public static extern nint GetDisplay(int display);
}