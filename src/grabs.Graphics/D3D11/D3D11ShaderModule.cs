using System.Diagnostics.CodeAnalysis;
using grabs.Core;
using grabs.Graphics.Exceptions;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;
using TerraFX.Interop.DirectX;
using static TerraFX.Interop.DirectX.DirectX;

namespace grabs.Graphics.D3D11;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
internal sealed unsafe class D3D11ShaderModule : ShaderModule
{
    public readonly ID3DBlob* Blob;

    public D3D11ShaderModule(ShaderStage stage, ref readonly ReadOnlySpan<byte> spirv, string entryPoint)
    {
        string hlsl = SpirvToHLSL(stage, in spirv, entryPoint);
        Console.WriteLine(hlsl);

        string target = stage switch
        {
            ShaderStage.Vertex => "vs_5_0",
            ShaderStage.Pixel => "ps_5_0",
            ShaderStage.Geometry => "gs_5_0",
            ShaderStage.Compute => "cs_5_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        using PinnedString pHlsl = new PinnedString(hlsl);
        using PinnedString pEntryPoint = new PinnedString("main");
        using PinnedString pTarget = new PinnedString(target);
        
        fixed (ID3DBlob** blob = &Blob)
        {
            ID3DBlob* errorBlob = null;
            if (D3DCompile((byte*) pHlsl, (nuint) hlsl.Length, null, null, null, pEntryPoint, pTarget, 0, 0, blob,
                    &errorBlob).FAILED)
            {
                throw new ShaderCompilationException(stage, new string((sbyte*) errorBlob->GetBufferPointer()));
            }

            if (errorBlob != null)
                errorBlob->Release();
        }
    }
    
    public override void Dispose()
    {
        Blob->Release();
    }

    private static readonly Cross _spirv;

    static D3D11ShaderModule()
    {
        _spirv = Cross.GetApi();
    }

    private static string SpirvToHLSL(ShaderStage stage, ref readonly ReadOnlySpan<byte> spirv, string entryPoint)
    {
        Context* context;
        _spirv.ContextCreate(&context);

        ParsedIr* ir;

        fixed (byte* spv = spirv)
        {
            if (_spirv.ContextParseSpirv(context, (uint*) spv, (nuint) (spirv.Length / sizeof(uint)), &ir) !=
                Result.Success)
            {
                throw new ShaderCompilationException(stage, _spirv.ContextGetLastErrorStringS(context));
            }
        }

        Compiler* compiler;
        _spirv.ContextCreateCompiler(context, Silk.NET.SPIRV.Cross.Backend.Hlsl, ir, CaptureMode.TakeOwnership,
            &compiler);

        CompilerOptions* options;
        _spirv.CompilerCreateCompilerOptions(compiler, &options);
        _spirv.CompilerOptionsSetUint(options, CompilerOption.HlslShaderModel, 50);
        _spirv.CompilerInstallCompilerOptions(compiler, options);

        ExecutionModel model = stage switch
        {
            ShaderStage.Vertex => ExecutionModel.Vertex,
            ShaderStage.Pixel => ExecutionModel.Fragment,
            ShaderStage.Geometry => ExecutionModel.Geometry,
            ShaderStage.Compute => ExecutionModel.GLCompute,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        _spirv.CompilerSetEntryPoint(compiler, entryPoint, model);

        byte* output;
        if (_spirv.CompilerCompile(compiler, &output) != Result.Success)
            throw new ShaderCompilationException(stage, _spirv.ContextGetLastErrorStringS(context));

        return new string((sbyte*) output);
    }
}