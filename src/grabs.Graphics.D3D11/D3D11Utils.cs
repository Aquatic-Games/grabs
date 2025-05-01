using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.Windows;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal static class D3D11Utils
{
    public static void Check(this HRESULT result, string operation)
    {
        if (result.FAILED)
            throw new Exception($"D3D11 operation '{operation}' failed: 0x{result.Value:x8}");
    }
}