using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace grabs.Core;

public unsafe struct PinnedStringArray : IDisposable
{
    private byte** _array;

    public readonly uint Length;

    public nint Handle => (nint) _array;

    public PinnedStringArray(params ReadOnlySpan<string> strings)
    {
        Length = (uint) strings.Length;

        if (Length == 0)
        {
            _array = null;
            return;
        }

        _array = (byte**) NativeMemory.Alloc((nuint) (Length * sizeof(byte*)));

        for (int i = 0; i < Length; i++)
        {
            byte[] bytes = Encoding.UTF8.GetBytes(strings[i]);

            // Allocate additional null byte.
            _array[i] = (byte*) NativeMemory.Alloc((nuint) ((bytes.Length + 1) * sizeof(byte)));
            _array[i][bytes.Length] = 0;

            fixed (byte* pBytes = bytes)
                Unsafe.CopyBlock(_array[i], pBytes, (uint) bytes.Length);
        }
    }
    
    public PinnedStringArray(List<string> strings) : this(CollectionsMarshal.AsSpan(strings)) { }

    public static implicit operator byte**(in PinnedStringArray array)
        => array._array;

    public static implicit operator PinnedStringArray(string[] array)
        => new PinnedStringArray(array);
    
    public void Dispose()
    {
        if (_array == null)
            return;
        
        for (int i = 0; i < Length; i++)
            NativeMemory.Free(_array[i]);
        
        NativeMemory.Free(_array);
    }
}