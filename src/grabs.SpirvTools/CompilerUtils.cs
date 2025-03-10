using grabs.Core;
using grabs.Graphics;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;

namespace grabs.SpirvTools;

public static class CompilerUtils
{
    private static readonly Cross _spirv;

    static CompilerUtils()
    {
        _spirv = Cross.GetApi();
    }
    
    public static unsafe string HlslFromSpirv(ShaderStage stage, in ReadOnlySpan<byte> spirv, string entryPoint)
    {
        Context* context;
        if (_spirv.ContextCreate(&context) != Result.Success)
            throw new Exception($"Failed to create context: {_spirv.ContextGetLastErrorStringS(context)}");

        try
        {
            ParsedIr* ir;
            fixed (byte* pSpirv = spirv)
            {
                if (_spirv.ContextParseSpirv(context, (uint*) pSpirv, (nuint) (spirv.Length / sizeof(uint)), &ir) !=
                    Result.Success)
                    throw new Exception($"Failed to parse spir-v: {_spirv.ContextGetLastErrorStringS(context)}");
            }

            Compiler* compiler;
            _spirv.ContextCreateCompiler(context, Backend.Hlsl, ir, CaptureMode.TakeOwnership, &compiler);

            CompilerOptions* options;
            _spirv.CompilerCreateCompilerOptions(compiler, &options);

            _spirv.CompilerOptionsSetUint(options, CompilerOption.HlslShaderModel, 50);
            _spirv.CompilerInstallCompilerOptions(compiler, options);

            ExecutionModel model = stage switch
            {
                ShaderStage.Vertex => ExecutionModel.Vertex,
                ShaderStage.Pixel => ExecutionModel.Fragment,
                _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
            };

            using PinnedString pEntryPoint = entryPoint;

            _spirv.CompilerSetEntryPoint(compiler, pEntryPoint, model);

            byte* source;
            if (_spirv.CompilerCompile(compiler, &source) != Result.Success)
                throw new Exception($"Failed to compile: {_spirv.ContextGetLastErrorStringS(context)}");

            string code = new string((sbyte*) source);

            return code;
        }
        finally
        {
            _spirv.ContextDestroy(context);
        }
    }
}