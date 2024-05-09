using System.Runtime.InteropServices;
using System.Text;

namespace grabs.Core;

public unsafe class PinnedString : IPinnedObject
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

    public override string ToString()
    {
        return new string((sbyte*) Handle);
    }

    public static implicit operator byte*(PinnedString pString)
        => (byte*) pString.Handle;
}