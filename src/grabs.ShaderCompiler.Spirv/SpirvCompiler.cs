using grabs.Graphics;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;

namespace grabs.ShaderCompiler.Spirv;

public class SpirvCompiler
{
    public static Cross Spirv;

    static SpirvCompiler()
    {
        Spirv = Cross.GetApi();
    }
    
    public static unsafe string TranspileSpirv(ShaderStage stage, Backend backend, byte[] spirv, string entryPoint)
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

        Compiler* compiler;
        if ((result = Spirv.ContextCreateCompiler(context, backend, ir, CaptureMode.TakeOwnership, &compiler)) != Result.Success)
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
}