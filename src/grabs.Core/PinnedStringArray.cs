using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace grabs.Core;

public unsafe class PinnedStringArray : IPinnedObject
{
    private byte** _stringPtrs;

    public nint Handle => (nint) _stringPtrs;

    public readonly uint Length;

    public PinnedStringArray(params string[] strings) : this(strings, Encoding.UTF8) { }

    public PinnedStringArray(string[] strings, Encoding encoding)
    {
        Length = (uint) strings.Length;
        
        _stringPtrs = (byte**) NativeMemory.Alloc((nuint) (strings.Length * sizeof(byte)));

        for (int i = 0; i < Length; i++)
        {
            byte[] bytes = encoding.GetBytes(strings[i]);
            if (bytes.Length == 0)
                continue;
            
            uint size = (uint) (bytes.Length + 1) * sizeof(byte);
            _stringPtrs[i] = (byte*) NativeMemory.Alloc(size);
            
            fixed (byte* pBytes = bytes)
                Unsafe.CopyBlock(_stringPtrs[i], pBytes, size);
        }
    }
    
    public void Dispose()
    {
        for (int i = 0; i < Length; i++)
            NativeMemory.Free(_stringPtrs[i]);
        
        NativeMemory.Free(_stringPtrs);
    }

    public override string ToString()
    {
        StringBuilder builder = new StringBuilder("[");

        for (int i = 0; i < Length; i++)
        {
            builder.Append(new string((sbyte*) _stringPtrs[i]));

            if (i < Length - 1)
                builder.Append(", ");
        }

        builder.Append(']');

        return builder.ToString();
    }
    
    public static implicit operator byte**(PinnedStringArray pStringArray)
        => (byte**) pStringArray.Handle;
}