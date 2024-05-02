using System.Runtime.InteropServices;
using System.Text;

namespace grabs.Core;

public struct PinnedString : IPinnedObject
{
    private GCHandle _gcHandle;

    public nint Handle => _gcHandle.AddrOfPinnedObject();

    public PinnedString(string @string) : this(@string, Encoding.UTF8) { }

    public PinnedString(string @string, Encoding encoding)
    {
        byte[] bytes = encoding.GetBytes(@string);
        _gcHandle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
    }
    
    public void Dispose()
    {
        _gcHandle.Free();
    }
}