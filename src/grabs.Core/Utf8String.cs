using System.Runtime.InteropServices;
using System.Text;

namespace grabs.Core;

public struct Utf8String : IDisposable
{
    private GCHandle _handle;

    public nint Handle => _handle.AddrOfPinnedObject();

    public Utf8String(string @string)
    {
        byte[] bytes = Encoding.UTF8.GetBytes(@string);
        _handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
    }
    
    public static implicit operator Utf8String(string @string)
        => new (@string);
    
    public static unsafe implicit operator byte*(in Utf8String utf8String)
        => (byte*) utf8String.Handle;

    public static unsafe implicit operator sbyte*(in Utf8String utf8String)
        => (sbyte*) utf8String.Handle;
    
    public void Dispose()
    {
        _handle.Free();
    }
}