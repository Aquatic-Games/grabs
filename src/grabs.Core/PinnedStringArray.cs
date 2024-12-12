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
        _array = (byte**) NativeMemory.Alloc((uint) (Length * sizeof(byte*)));
        
        for (int i = 0; i < Length; i++)
        {
            uint stringLength = (uint) strings[i].Length + 1;
            _array[i] = (byte*) NativeMemory.Alloc(stringLength * sizeof(byte));

            byte[] stringBytes = Encoding.UTF8.GetBytes(strings[i]);
            
            fixed (byte* pString = stringBytes)
                Unsafe.CopyBlock(_array[i], pString, stringLength);

            _array[i][stringLength - 1] = 0;
        }
    }

    public static implicit operator byte**(in PinnedStringArray array)
        => array._array;

    public override string ToString()
    {
        StringBuilder sb = new StringBuilder("[");

        for (int i = 0; i < Length; i++)
        {
            sb.Append(new string((sbyte*) _array[i]));

            if (i < Length - 1)
                sb.Append(", ");
        }

        sb.Append(']');

        return sb.ToString();
    }

    public void Dispose()
    {
        for (int i = 0; i < Length; i++)
            NativeMemory.Free(_array[i]);
        
        NativeMemory.Free(_array);
    }
    
    
}