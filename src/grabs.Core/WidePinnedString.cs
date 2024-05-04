using System.Runtime.InteropServices;

namespace grabs.Core;

public class WidePinnedString : IPinnedObject
{
    private GCHandle _handle;

    public nint Handle => _handle.AddrOfPinnedObject();

    public WidePinnedString(string @string)
    {
        char[] chars = @string.ToCharArray();
        
        if (OperatingSystem.IsWindows())
        {
            _handle = GCHandle.Alloc(chars, GCHandleType.Pinned);
        }
        else
        {
            int[] ints = Array.ConvertAll(chars, input => (int) input);
            _handle = GCHandle.Alloc(ints, GCHandleType.Pinned);
        }
    }
    
    public void Dispose()
    {
        _handle.Free();
    }

    public static unsafe implicit operator char*(WidePinnedString pString)
        => (char*) pString.Handle;
}