using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace grabs.Core;

public unsafe struct PinnedStringArray : IPinnedObject
{
    private byte** _stringPtrs;

    public nint Handle => (nint) _stringPtrs;

    public int Length;

    public PinnedStringArray(params string[] strings) : this(strings, Encoding.UTF8) { }

    public PinnedStringArray(string[] strings, Encoding encoding)
    {
        Length = strings.Length;
        
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
            builder.Append(", ");
        }

        builder.Append(']');

        return builder.ToString();
    }

    public static PinnedStringArray FromChars(params string[] strings)
    {
        int length = strings.Length;
        byte** stringPtrs = (byte**) NativeMemory.Alloc((nuint) (length * sizeof(char*)));

        for (int i = 0; i < length; i++)
        {
            char[] chars = strings[i].ToCharArray();
            if (chars.Length == 0)
            {
                stringPtrs[i] = null;
                continue;
            }

            uint size = (uint) (chars.Length + 1) * sizeof(char);
            stringPtrs[i] = (byte*) NativeMemory.Alloc(size);
            
            fixed (char* pChars = chars)
                Unsafe.CopyBlock(stringPtrs[i], pChars, size);
        }

        return new PinnedStringArray()
        {
            _stringPtrs = stringPtrs,
            Length = length
        };
    }
    
    public static implicit operator byte**(PinnedStringArray pStringArray)
        => (byte**) pStringArray.Handle;
}