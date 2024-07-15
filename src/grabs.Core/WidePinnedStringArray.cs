using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace grabs.Core;

public unsafe struct WidePinnedStringArray : IPinnedObject
{
    private void** _stringPtrs;
    
    public nint Handle => (nint) _stringPtrs;

    public readonly uint Length;

    public WidePinnedStringArray(params string[] strings)
    {
        Length = (uint) strings.Length;

        uint charSize = (uint) (OperatingSystem.IsWindows() ? sizeof(char) : sizeof(int));

        _stringPtrs = (void**) NativeMemory.Alloc((nuint) (strings.Length * sizeof(void*)));

        for (int i = 0; i < Length; i++)
        {
            char[] chars = strings[i].ToCharArray();
            
            uint size = (uint) (chars.Length + 1) * charSize;
            _stringPtrs[i] = NativeMemory.Alloc(size);

            if (OperatingSystem.IsWindows())
            {
                fixed (char* pChars = chars)
                    Unsafe.CopyBlock(_stringPtrs[i], pChars, size);
            }
            else
            {
                int[] ints = Array.ConvertAll(chars, input => (int) input);
                
                fixed (int* pInts = ints)
                    Unsafe.CopyBlock(_stringPtrs[i], pInts, size);
            }
        }
    }
    
    public void Dispose()
    {
        for (int i = 0; i < Length; i++)
            NativeMemory.Free(_stringPtrs[i]);
        
        NativeMemory.Free(_stringPtrs);
    }
    
    public static implicit operator char**(WidePinnedStringArray pStringArray)
        => (char**) pStringArray.Handle;
}