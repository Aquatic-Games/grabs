using grabs.Core;
using grabs.Graphics;
using Silk.NET.SPIRV;
using Silk.NET.SPIRV.Cross;

namespace grabs.SpirvTools;

public static unsafe class CompilerUtils
{
    public const uint PushConstantBindingPoint = 0;
    
    private static readonly Cross _spirv;

    static CompilerUtils()
    {
        _spirv = Cross.GetApi();
    }
    
    public static string HlslFromSpirv(ShaderStage stage, in ReadOnlySpan<byte> spirv, string entryPoint, out Dictionary<uint, Dictionary<uint, uint>> remappings)
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

            uint id;
            //_spirv.CompilerBuildDummySamplerForCombinedImages(compiler, &id);
            //_spirv.CompilerBuildCombinedImageSamplers(compiler);
            
            //Console.WriteLine(id);

            CombinedImageSampler* sampled;
            nuint numImages;
            _spirv.CompilerGetCombinedImageSamplers(compiler, &sampled, &numImages);
            
            for (uint i = 0; i < numImages; i++)
            {
                Console.WriteLine(
                    $"combid: {sampled[i].CombinedId}, sid: {sampled[i].SamplerId}, iid: {sampled[i].ImageId}, set: {_spirv.CompilerGetDecoration(compiler, sampled[i].CombinedId, Decoration.Binding)}");
            }

            remappings = new Dictionary<uint, Dictionary<uint, uint>>();

            Resources* resources;
            _spirv.CompilerCreateShaderResources(compiler, &resources);
            RemapType(compiler, resources, ResourceType.UniformBuffer, 1, ref remappings);
            RemapType(compiler, resources, ResourceType.SeparateImage, 0, ref remappings);
            RemapType(compiler, resources, ResourceType.SeparateSamplers, 0, ref remappings);
            RemapType(compiler, resources, ResourceType.PushConstant, PushConstantBindingPoint, ref remappings);

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

    private static void RemapType(Compiler* compiler, Resources* resources, ResourceType type, uint startOffset, ref Dictionary<uint, Dictionary<uint, uint>> remappings)
    {
        ReflectedResource* reflectedResources;
        nuint numResources;
        _spirv.ResourcesGetResourceListForType(resources, type, &reflectedResources, &numResources);

        uint bindNum = startOffset;
        for (uint i = 0; i < numResources; i++)
        {
            uint id = reflectedResources[i].Id;
            Console.WriteLine(id);

            uint set = _spirv.CompilerGetDecoration(compiler, id, Decoration.DescriptorSet);
            uint binding = _spirv.CompilerGetDecoration(compiler, id, Decoration.Binding);

            uint newBinding = bindNum++;
            _spirv.CompilerSetDecoration(compiler, id, Decoration.DescriptorSet, 0);
            _spirv.CompilerSetDecoration(compiler, id, Decoration.Binding, newBinding);

            if (!remappings.TryGetValue(set, out Dictionary<uint, uint> remapping))
            {
                remapping = new Dictionary<uint, uint>();
                remappings[set] = remapping;
            }

            remapping[binding] = newBinding;
        }
    }
}