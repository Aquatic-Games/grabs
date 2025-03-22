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

    public const int None = 0x3038;

    public const int ContextMajorVersion = 0x3098;
    public const int ContextMinorVersion = 0x30FB;
    public const int ContextOpenGlProfileMask = 0x30FD;

    public const int ContextOpenGlCoreProfileBit = 1;

    public const int OpenGlApi = 0x30A2;

    [DllImport(DllName, EntryPoint = "eglGetError")]
    public static extern Result GetError();
    
    [DllImport(DllName, EntryPoint = "eglGetDisplay")]
    public static extern void* GetDisplay(int display);

    [DllImport(DllName, EntryPoint = "eglInitialize")]
    public static extern bool Initialize(void* display, int* major, int* minor);

    [DllImport(DllName, EntryPoint = "eglChooseConfig")]
    public static extern bool ChooseConfig(void* dpy, int* attribList, void** configs, int configSize, int* numConfigs);

    [DllImport(DllName, EntryPoint = "eglBindAPI")]
    public static extern bool BindAPI(int api);

    [DllImport(DllName, EntryPoint = "eglCreateContext")]
    public static extern void* CreateContext(void* display, void* config, void* shareContext, int* attribList);

    [DllImport(DllName, EntryPoint = "eglCreatePlatformWindowSurface")]
    public static extern void* CreatePlatformWindowSurface(void* display, void* config, void* window, int* attribList);

    [DllImport(DllName, EntryPoint = "eglMakeCurrent")]
    public static extern bool MakeCurrent(void* display, void* draw, void* read, void* context);

    [DllImport(DllName, EntryPoint = "eglGetProcAddress")]
    public static extern void* GetProcAddress(sbyte* procname);

    public enum Result
    {
        Success = 0x3000,
        NotInitialized = 0x3001,
        BadAccess = 0x3002,
        BadAlloc = 0x3003,
        BadAttribute = 0x3004,
        BadConfig = 0x3005,
        BadContext = 0x3006,
        BadCurrentSurface = 0x3007,
        BadDisplay = 0x3008,
        BadMatch = 0x3009,
        BadNativePixmap = 0x300A,
        BadNativeWindow = 0x300B,
        BadParameter = 0x300C,
        BadSurface = 0x300D,
        ContextLost = 0x300E
    }
}