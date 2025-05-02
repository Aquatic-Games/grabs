using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace grabs.Graphics.D3D11;

internal sealed unsafe class D3D11ShaderModule : ShaderModule
{
    public override bool IsDisposed { get; protected set; }

    public readonly byte* Code;

    public readonly nuint CodeLength;

    public D3D11ShaderModule(byte[] code)
    {
        CodeLength = (nuint) code.Length;

        Code = (byte*) NativeMemory.Alloc(CodeLength);

        fixed (byte* pCode = code)
            Unsafe.CopyBlock(Code, pCode, (uint) CodeLength);
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
        
        NativeMemory.Free(Code);
    }
}