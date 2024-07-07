using System;
using grabs.ShaderCompiler.Spirv;
using Vortice.D3DCompiler;
using Vortice.Direct3D;

namespace grabs.Graphics.D3D11;

public sealed class D3D11ShaderModule : ShaderModule
{
    public Blob Blob;
    
    public D3D11ShaderModule(ShaderStage stage, byte[] spirv, string entryPoint, SpecializationConstant[] constants) 
        : base(stage)
    {
        string hlsl = SpirvCompiler.TranspileSpirv(stage, ShaderLanguage.Hlsl50, spirv, entryPoint, constants);

        string profile = stage switch
        {
            ShaderStage.Vertex => "vs_5_0",
            ShaderStage.Pixel => "ps_5_0",
            ShaderStage.Compute => "cs_5_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
        
        Blob errorBlob;
        if (Compiler.Compile(hlsl, null, null, "main", null, profile, ShaderFlags.None, EffectFlags.None, out Blob, out errorBlob).Failure)
            throw new Exception($"Failed to compile HLSL shader: {errorBlob.AsString()}");
        
        errorBlob?.Dispose();
    }
    
    public override void Dispose()
    {
        Blob.Dispose();
    }
}