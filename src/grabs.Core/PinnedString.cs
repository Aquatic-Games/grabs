using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace grabs.Core;

public unsafe struct PinnedString : IDisposable
{
    private GCHandle _handle;

    public nint Handle => _handle.AddrOfPinnedObject();

    public PinnedString(string @string, Encoding encoding)
    {
        byte[] bytes = encoding.GetBytes(@string);
        _handle = GCHandle.Alloc(bytes, GCHandleType.Pinned);
    }

    public PinnedString(string @string) : this(@string, Encoding.UTF8) { }

    public static explicit operator byte*(in PinnedString @string)
        => (byte*) @string.Handle;

    public override string ToString()
    {
        return new string((sbyte*) (byte*) this);
    }

    public void Dispose()
    {
        _handle.Free();
    }
}