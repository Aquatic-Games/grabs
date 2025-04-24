using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace grabs.Core;

public unsafe struct Utf8Array : IDisposable
{
    private readonly byte** _array;

    public readonly uint Length;

    public nint Handle => (nint) _array;

    public Utf8Array(params ReadOnlySpan<string> strings)
    {
        Length = (uint) strings.Length;

        // Avoid allocating if there are no strings.
        if (Length == 0)
            return;

        _array = (byte**) NativeMemory.Alloc((nuint) (Length * sizeof(byte**)));

        for (uint i = 0; i < Length; i++)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(strings[(int) i]);
            _array[i] = (byte*) NativeMemory.Alloc((nuint) bytes.Length + 1);
            // Ensure null byte is present. 
            _array[i][bytes.Length] = 0;
            
            fixed (byte* pBytes = bytes)
                Unsafe.CopyBlockUnaligned(_array[i], pBytes, (uint) bytes.Length);
        }
    }

    public Utf8Array(List<string> stringList) : this(CollectionsMarshal.AsSpan(stringList)) { }

    public static unsafe implicit operator byte**(in Utf8Array array)
        => array._array;

    public void Dispose()
    {
        if (Length == 0)
            return;
        
        for (uint i = 0; i < Length; i++)
            NativeMemory.Free(_array[i]);
        
        NativeMemory.Free(_array);
    }
}