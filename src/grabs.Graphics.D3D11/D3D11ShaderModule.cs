using System;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;
using Vortice.D3DCompiler;
using Vortice.Direct3D;
using Compiler = Vortice.D3DCompiler.Compiler;
using SpvCompiler = Silk.NET.SPIRV.Cross.Compiler;

namespace grabs.Graphics.D3D11;

public sealed class D3D11ShaderModule : ShaderModule
{
    public static Cross Spirv;

    static D3D11ShaderModule()
    {
        Spirv = Cross.GetApi();
    }

    public static unsafe string SpirVToHLSL(ShaderStage stage, byte[] spirv, string entryPoint)
    {
        Result result;
        
        Context* context;
        if ((result = Spirv.ContextCreate(&context)) != Result.Success)
            throw new Exception($"Failed to create context: {result} - {Spirv.ContextGetLastErrorStringS(context)}");

        ParsedIr* ir;
        fixed (byte* pSpirv = spirv)
        {
            if ((result = Spirv.ContextParseSpirv(context, (uint*) pSpirv, (nuint) (spirv.Length / sizeof(uint)), &ir)) != Result.Success)
                throw new Exception($"Failed to parse SPIRV: {result} - {Spirv.ContextGetLastErrorStringS(context)}");
        }

        SpvCompiler* compiler;
        if ((result = Spirv.ContextCreateCompiler(context, Backend.Hlsl, ir, CaptureMode.TakeOwnership, &compiler)) != Result.Success)
            throw new Exception($"Failed to create compiler: {result} - {Spirv.ContextGetLastErrorStringS(context)}");

        CompilerOptions* options;
        if ((result = Spirv.CompilerCreateCompilerOptions(compiler, &options)) != Result.Success)
            throw new Exception($"Failed to create compiler options: {result} - {Spirv.ContextGetLastErrorStringS(context)}");

        ExecutionModel model = stage switch
        {
            ShaderStage.Vertex => ExecutionModel.Vertex,
            ShaderStage.Pixel => ExecutionModel.Fragment,
            ShaderStage.Compute => ExecutionModel.GLCompute, // GL compute?
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        Spirv.CompilerSetEntryPoint(compiler, entryPoint, model);

        // Use shader model 5.0
        Spirv.CompilerOptionsSetUint(options, CompilerOption.HlslShaderModel, 50);
        Spirv.CompilerInstallCompilerOptions(compiler, options);

        byte* pStrResult;
        if ((result = Spirv.CompilerCompile(compiler, &pStrResult)) != Result.Success)
            throw new Exception($"Failed to compile SPIRV shader: {result} - {Spirv.ContextGetLastErrorStringS(context)}");

        string strResult = new string((sbyte*) pStrResult);
        
        Spirv.ContextReleaseAllocations(context);
        Spirv.ContextDestroy(context);

        return strResult;
    }
    
    public Blob Blob;
    
    public D3D11ShaderModule(ShaderStage stage, byte[] spirv, string entryPoint) : base(stage)
    {
        string hlsl = SpirVToHLSL(stage, spirv, entryPoint);

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