using System;
using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using grabs.ShaderCompiler.Spirv;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public sealed unsafe class D3D11ShaderModule : ShaderModule
{
    public readonly ID3DBlob* Blob;
    
    public readonly DescriptorRemappings DescriptorRemappings;
    
    public D3D11ShaderModule(ShaderStage stage, byte[] spirv, string entryPoint, SpecializationConstant[] constants) 
        : base(stage)
    {
        string hlsl = SpirvCompiler.TranspileSpirv(stage, ShaderLanguage.Hlsl50, spirv, entryPoint,
            out DescriptorRemappings, constants);

        string profile = stage switch
        {
            ShaderStage.Vertex => "vs_5_0",
            ShaderStage.Pixel => "ps_5_0",
            ShaderStage.Compute => "cs_5_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        using PinnedString pHlsl = new PinnedString(hlsl);
        using PinnedString pEntryPoint = new PinnedString("main");
        using PinnedString pProfile = new PinnedString(profile);
        
        ID3DBlob* errorBlob;
        fixed (ID3DBlob** blob = &Blob)
        {
            if (D3DCompile(pHlsl, (nuint) hlsl.Length, null, null, null, (sbyte*) pEntryPoint.Handle,
                    (sbyte*) pProfile.Handle, 0, 0, blob, &errorBlob).FAILED)
            {
                throw new Exception(
                    $"Failed to compile HLSL shader: {new string((sbyte*) errorBlob->GetBufferPointer(), 0, (int) errorBlob->GetBufferSize())}");
            }
        }

        if (errorBlob != null)
            errorBlob->Release();
    }
    
    public override void Dispose()
    {
        Blob->Release();
    }
}