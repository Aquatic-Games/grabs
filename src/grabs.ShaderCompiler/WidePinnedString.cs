using System.Runtime.InteropServices;

namespace grabs.ShaderCompiler;

public struct WidePinnedString : IDisposable
{
    private GCHandle _handle;

    public nint Handle => _handle.AddrOfPinnedObject();

    public WidePinnedString(string @string)
    {
        char[] chars = @string.ToCharArray();
        
        // On Windows platforms DXC is compiled with 2-byte chars, however on Linux platforms it seems to be compiled
        // with 4-byte chars.
        if (OperatingSystem.IsWindows())
            _handle = GCHandle.Alloc(chars, GCHandleType.Pinned);
        else
        {
            int[] ints = Array.ConvertAll(chars, input => (int) input);
            _handle = GCHandle.Alloc(ints, GCHandleType.Pinned);
        }
    }

    public static unsafe implicit operator char*(in WidePinnedString widePinnedString)
        => (char*) widePinnedString.Handle;

    public static implicit operator WidePinnedString(string @string)
        => new WidePinnedString(@string);
    
    public void Dispose()
    {
        _handle.Free();
    }
}