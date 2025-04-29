using System.Diagnostics.CodeAnalysis;
using TerraFX.Interop.Windows;

namespace grabs.Graphics.D3D12;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal static class D3D12Utils
{
    public static void Check(this HRESULT result, string operation)
    {
        if (result.FAILED)
            throw new Exception($"D3D12 operation '{operation}' failed with HRESULT: 0x{result.Value:X8}");
    }
}