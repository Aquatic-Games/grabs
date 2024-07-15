using System;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using TerraFX.Interop.Windows;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static class D3DResult
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void CheckResult(HRESULT result, string operation)
    {
        if (result.FAILED)
            throw new Exception($"Operation '{operation}' failed with HRESULT {result}.");
    }
    
    public static void CheckResult(HRESULT result)
    {
        if (result.FAILED)
            throw new Exception($"Operation failed with HRESULT {result}.");
    }
}