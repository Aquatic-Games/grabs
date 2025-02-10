using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace grabs.ShaderCompiler;

public unsafe struct WidePinnedStringArray : IDisposable
{
    private readonly char** _array;

    public readonly uint Length;

    public nint Handle => (nint) _array;

    public WidePinnedStringArray(params ReadOnlySpan<string> strings)
    {
        Length = (uint) strings.Length;

        // On Windows platforms DXC is compiled with 2-byte chars, however on Linux platforms it seems to be compiled
        // with 4-byte chars.
        if (OperatingSystem.IsWindows())
        {
            _array = (char**) NativeMemory.Alloc((nuint) (Length * sizeof(char*)));

            for (int i = 0; i < Length; i++)
            {
                char[] chars = strings[i].ToCharArray();
                
                _array[i] = (char*) NativeMemory.Alloc((nuint) ((chars.Length + 1) * sizeof(char)));
                _array[i][chars.Length] = '\0'; // Ensure null byte is present
                
                fixed (char* pChars = chars)
                    Unsafe.CopyBlock(_array[i], pChars, (uint) chars.Length * sizeof(char));
            }
        }
        else
        {
            _array = (char**) NativeMemory.Alloc((nuint) (Length * sizeof(int*)));

            for (int i = 0; i < Length; i++)
            {
                char[] chars = strings[i].ToCharArray();
                int[] ints = Array.ConvertAll(chars, input => (int) input);

                _array[i] = (char*) NativeMemory.Alloc((nuint) ((ints.Length + 1) * sizeof(int)));
                ((int*) _array[i])[ints.Length] = 0; // Ensure null byte, but we must cast the array to int*
                
                fixed (int* pInts = ints)
                    Unsafe.CopyBlock(_array[i], pInts, (uint) (ints.Length * sizeof(int)));
            }
        }
    }

    public WidePinnedStringArray(List<string> list) : this(CollectionsMarshal.AsSpan(list)) { }

    public static implicit operator char**(in WidePinnedStringArray array)
        => array._array;

    public static implicit operator WidePinnedStringArray(string[] array)
        => new WidePinnedStringArray(array);

    public void Dispose()
    {
        for (int i = 0; i < Length; i++)
            NativeMemory.Free(_array[i]);
        
        NativeMemory.Free(_array);
    }
}