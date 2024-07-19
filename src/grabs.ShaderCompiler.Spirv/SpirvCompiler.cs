using System;
using grabs.Graphics;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;
using SpecConstant = Silk.NET.SPIRV.Cross.SpecializationConstant;
using SpecializationConstant = grabs.Graphics.SpecializationConstant;

namespace grabs.ShaderCompiler.Spirv;

public class SpirvCompiler
{
    public static Cross Spirv;

    static SpirvCompiler()
    {
        Spirv = Cross.GetApi();
    }
    
    public static unsafe string TranspileSpirv(ShaderStage stage, ShaderLanguage language, byte[] spirv,
        string entryPoint, SpecializationConstant[] constants = null)
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

        // DX and GL don't support descriptor sets - so we need to fake it.
        // We "flatten" the descriptor sets, i.e. if set 0 has bindings 0 and 1, and set 1 has binding 0, then we instead
        // make set 1 have a binding of 2, etc.
        Resources* resources;
        Spirv.CompilerCreateShaderResources(compiler, &resources);

        uint bindIndex = 0;
        ChangeDescriptorBindingsForType(compiler, resources, ResourceType.UniformBuffer, ref bindIndex);
        ChangeDescriptorBindingsForType(compiler, resources, ResourceType.SeparateImage, ref bindIndex);
        ChangeDescriptorBindingsForType(compiler, resources, ResourceType.SeparateSamplers, ref bindIndex);
        
        // TODO: Combined image samplers need to be reimplemented. This should be tested with GLSL though, as I can't figure out DXC's combined samplers right now. Something is broken here!
        /*// I have absolutely no idea how I figured this out. Copied directly from Pie's compiler.
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
        }*/

        if (constants != null)
        {
            SpecConstant* specConstants;
            nuint numConstants;
            Spirv.CompilerGetSpecializationConstants(compiler, &specConstants, &numConstants);

            foreach (SpecializationConstant constant in constants)
            {
                // TODO: Horribly inefficient. Change please!
                for (int i = 0; i < (int) numConstants; i++)
                {
                    if (specConstants[i].ConstantId == constant.Id)
                    {
                        Constant* c = Spirv.CompilerGetConstantHandle(compiler, specConstants[i].Id);
                        
                        switch (constant.Type)
                        {
                            case ConstantType.I8:
                                Spirv.ConstantSetScalarI8(c, 0, 0, (byte) constant.Value);
                                break;
                            case ConstantType.I16:
                                Spirv.ConstantSetScalarI16(c, 0, 0, (short) constant.Value);
                                break;
                            case ConstantType.I32:
                                Spirv.ConstantSetScalarI32(c, 0, 0, (int) constant.Value);
                                break;
                            case ConstantType.I64:
                                Spirv.ConstantSetScalarI64(c, 0, 0, (long) constant.Value);
                                break;
                            case ConstantType.U8:
                                Spirv.ConstantSetScalarU8(c, 0, 0, (byte) constant.Value);
                                break;
                            case ConstantType.U16:
                                Spirv.ConstantSetScalarI16(c, 0, 0, (short) constant.Value);
                                break;
                            case ConstantType.U32:
                                Spirv.ConstantSetScalarU32(c, 0, 0, (uint) constant.Value);
                                break;
                            case ConstantType.U64:
                                Spirv.ConstantSetScalarU64(c, 0, 0, (ulong) constant.Value);
                                break;
                            case ConstantType.F32:
                                Spirv.ConstantSetScalarFp32(c, 0, 0, *(float*) &constant.Value);
                                break;
                            case ConstantType.F64:
                                Spirv.ConstantSetScalarFp64(c, 0, 0, *(double*) &constant.Value);
                                break;
                            default:
                                throw new ArgumentOutOfRangeException();
                        }
                    }
                }
            }
        }
        
        byte* pStrResult;
        if ((result = Spirv.CompilerCompile(compiler, &pStrResult)) != Result.Success)
            throw new Exception($"Failed to compile SPIRV shader: {result} - {Spirv.ContextGetLastErrorStringS(context)}");

        string strResult = new string((sbyte*) pStrResult);
        
        Spirv.ContextReleaseAllocations(context);
        Spirv.ContextDestroy(context);
        
        Console.WriteLine(strResult);

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