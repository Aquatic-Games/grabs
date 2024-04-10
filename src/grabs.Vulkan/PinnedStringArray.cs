using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;

namespace grabs.Vulkan;

public unsafe struct PinnedStringArray : IDisposable
{
    private byte** _data;

    public readonly int Length;

    public nint Handle => (nint) _data;

    public PinnedStringArray(params string[] strings)
    {
        Length = strings.Length;

        _data = (byte**) NativeMemory.Alloc((nuint) (Length * sizeof(byte*)));

        for (int i = 0; i < strings.Length; i++)
        {
            _data[i] = (byte*) NativeMemory.Alloc((nuint) strings[i].Length + 1);
            fixed (byte* pStr = Encoding.UTF8.GetBytes(strings[i]))
                Unsafe.CopyBlock(_data[i], pStr, (uint) strings[i].Length + 1);
        }
    }

    public void Dispose()
    {
        for (int i = 0; i < Length; i++)
            NativeMemory.Free(_data[i]);
        
        NativeMemory.Free(_data);
    }
}