using System;
using System.Collections.Generic;
using System.Linq;
using grabs.Core;
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
        string entryPoint, out DescriptorRemappings remappings, SpecializationConstant[] constants = null)
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

        remappings = new DescriptorRemappings();
        uint newBinding = 0;

        switch (language)
        {
            case ShaderLanguage.Hlsl50:
            {
                RemapDescriptorBindingsForType(compiler, resources, ResourceType.UniformBuffer, ref newBinding, ref remappings);
                newBinding = 0;
                RemapDescriptorBindingsForType(compiler, resources, ResourceType.SampledImage, ref newBinding, ref remappings);
                uint currentBinding = newBinding;
                RemapDescriptorBindingsForType(compiler, resources, ResourceType.SeparateSamplers, ref newBinding, ref remappings);
                newBinding = currentBinding;
                RemapDescriptorBindingsForType(compiler, resources, ResourceType.SeparateImage, ref newBinding, ref remappings);
                break;
            }
            case ShaderLanguage.Glsl430:
            {
                RemapDescriptorBindingsForType(compiler, resources, ResourceType.UniformBuffer, ref newBinding, ref remappings);
                RemapDescriptorBindingsForType(compiler, resources, ResourceType.SampledImage, ref newBinding, ref remappings);
                
                Spirv.CompilerBuildCombinedImageSamplers(compiler);
                
                CombinedImageSampler* combinedSamplers;
                nuint numSamplers;
                Spirv.CompilerGetCombinedImageSamplers(compiler, &combinedSamplers, &numSamplers);
                for (uint i = 0; i < (int) numSamplers; i++)
                {
                    uint id = combinedSamplers[i].ImageId;

                    uint set = Spirv.CompilerGetDecoration(compiler, id, Decoration.DescriptorSet);
                    uint binding = Spirv.CompilerGetDecoration(compiler, id, Decoration.Binding);
                    
                    Spirv.CompilerSetDecoration(compiler, combinedSamplers[i].CombinedId, Decoration.Binding, newBinding);
                    
                    AddRemapping(ref remappings, set, binding, newBinding);

                    newBinding++;
                }

                break;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(language), language, null);
        }

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
        
        GrabsLog.Log(GrabsLog.Severity.Verbose, strResult);

        return strResult;
    }

    private static unsafe void RemapDescriptorBindingsForType(Compiler* compiler, Resources* resources, ResourceType type, ref uint newBinding, ref DescriptorRemappings remappings)
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

            uint id = resourceList[i].Id;

            uint currentSet = Spirv.CompilerGetDecoration(compiler, id, Decoration.DescriptorSet);
            uint currentBinding = Spirv.CompilerGetDecoration(compiler, id, Decoration.Binding);
            
            Spirv.CompilerUnsetDecoration(compiler, id, Decoration.DescriptorSet);
            Spirv.CompilerSetDecoration(compiler, id, Decoration.Binding, newBinding);

            AddRemapping(ref remappings, currentSet, currentBinding, newBinding);
            
            newBinding++;
        }
    }

    private static void AddRemapping(ref DescriptorRemappings remappings, uint set, uint binding, uint newBinding)
    {
        if (!remappings.Sets.TryGetValue(set, out Remapping remapping))
        {
            remapping = new Remapping();
            remappings.Sets.Add(set, remapping);
        }
            
        remapping.Bindings.Add(binding, newBinding);
    }
}