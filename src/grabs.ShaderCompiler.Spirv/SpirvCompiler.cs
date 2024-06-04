using System;
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
    
    public static unsafe string TranspileSpirv(ShaderStage stage, ShaderLanguage language, byte[] spirv, string entryPoint)
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

        Backend backend = language switch
        {
            ShaderLanguage.Hlsl50 => Backend.Hlsl,
            ShaderLanguage.Glsl430 => Backend.Glsl,
            _ => throw new ArgumentOutOfRangeException(nameof(language), language, null)
        };
        
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

        switch (language)
        {
            case ShaderLanguage.Hlsl50:
                // Use shader model 5.0
                Spirv.CompilerOptionsSetUint(options, CompilerOption.HlslShaderModel, 50);
                break;
            case ShaderLanguage.Glsl430:
                Spirv.CompilerOptionsSetUint(options, CompilerOption.GlslVersion, 430);
                break;
            default:
                throw new ArgumentOutOfRangeException(nameof(language), language, null);
        }
        
        Spirv.CompilerInstallCompilerOptions(compiler, options);

        // I have absolutely no idea how I figured this out. Copied directly from Pie's compiler.
        uint id;
        Spirv.CompilerBuildDummySamplerForCombinedImages(compiler, &id);
        Spirv.CompilerBuildCombinedImageSamplers(compiler);

        CombinedImageSampler* samplers;
        nuint numSamplers;
        Spirv.CompilerGetCombinedImageSamplers(compiler, &samplers, &numSamplers);

        for (uint i = 0; i < numSamplers; i++)
        {
            uint decoration = Spirv.CompilerGetDecoration(compiler, samplers[i].ImageId, Decoration.Binding);
            Spirv.CompilerSetDecoration(compiler, samplers[i].CombinedId, Decoration.Binding, decoration);
        }

        Resources* resources;
        Spirv.CompilerCreateShaderResources(compiler, &resources);

        uint bindIndex = 0;
        ChangeDescriptorBindingsForType(compiler, resources, ResourceType.UniformBuffer, ref bindIndex);
        ChangeDescriptorBindingsForType(compiler, resources, ResourceType.SampledImage, ref bindIndex);

        byte* pStrResult;
        if ((result = Spirv.CompilerCompile(compiler, &pStrResult)) != Result.Success)
            throw new Exception($"Failed to compile SPIRV shader: {result} - {Spirv.ContextGetLastErrorStringS(context)}");

        string strResult = new string((sbyte*) pStrResult);
        
        Spirv.ContextReleaseAllocations(context);
        Spirv.ContextDestroy(context);

        return strResult;
    }

    private static unsafe void ChangeDescriptorBindingsForType(Compiler* compiler, Resources* resources, ResourceType type, ref uint currentIndex)
    {
        nuint numResources;
        ReflectedResource* resourceList;
        Spirv.ResourcesGetResourceListForType(resources, type, &resourceList, &numResources);

        for (int i = 0; i < (int) numResources; i++)
        {
            /*Console.WriteLine(new string((sbyte*) resourceList[i].Name));
            Console.WriteLine(resourceList[i].Id);
            Console.WriteLine(resourceList[i].TypeId);
            Console.WriteLine(resourceList[i].BaseTypeId);

            Console.WriteLine("Set:" + Spirv.CompilerGetDecoration(compiler, resourceList[i].Id, Decoration.DescriptorSet));
            Console.WriteLine("Binding:" + Spirv.CompilerGetDecoration(compiler, resourceList[i].Id, Decoration.Binding));
            
            Console.WriteLine();*/
            
            Spirv.CompilerUnsetDecoration(compiler, resourceList[i].Id, Decoration.DescriptorSet);
            Spirv.CompilerSetDecoration(compiler, resourceList[i].Id, Decoration.Binding, currentIndex++);
        }
    }
}