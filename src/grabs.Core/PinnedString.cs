using System.Runtime.InteropServices;
using System.Text;

namespace grabs.Core;

public struct PinnedString : IDisposable
{
    private GCHandle _handle;

    public nint Handle => _handle.AddrOfPinnedObject();
    
    public PinnedString(string @string, Encoding encoding)
    {
        byte[] bytes = encoding.GetBytes(@string);
        _handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
    }

    public PinnedString(string @string) : this(@string, Encoding.UTF8) { }

    public static unsafe implicit operator byte*(in PinnedString pinnedString)
        => (byte*) pinnedString.Handle;

    public static implicit operator PinnedString(string @string)
        => new PinnedString(@string);
    
    public void Dispose()
    {
        _handle.Free();
    }
}