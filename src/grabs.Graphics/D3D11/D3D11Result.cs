using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.Windows;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal static class D3D11Result
{
    public static void CheckResult(HRESULT result, string operation)
    {
        if (result.FAILED)
            throw new Exception($"D3D11 Operation '{operation}' failed with result code: {result.Value:X}");
    }
}