using System.Runtime.InteropServices;

namespace grabs.Graphics.OpenGL;

internal static unsafe class Egl
{
    public const string DllName = "EGL";

    public const int DefaultDisplay = 0;

    public const int AlphaSize = 0x3021;
    public const int GreenSize = 0x3023;
    public const int BlueSize = 0x3022;
    public const int RedSize = 0x3024;
    public const int DepthSize = 0x3025;
    public const int StencilSize = 0x3026;
    
    [DllImport(DllName, EntryPoint = "eglGetDisplay")]
    public static extern void* GetDisplay(int display);

    [DllImport(DllName, EntryPoint = "eglInitialize")]
    public static extern bool Initialize(void* display, int* major, int* minor);

    [DllImport(DllName, EntryPoint = "eglChooseConfig")]
    public static extern bool ChooseConfig(void* dpy, int* attribList, void** configs, int configSize, int* numConfigs);
}