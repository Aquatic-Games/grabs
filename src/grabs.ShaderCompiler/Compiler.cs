using grabs.Core;
using Vortice.Dxc;
using static Vortice.Dxc.Dxc;

namespace grabs.ShaderCompiler;

public static class Compiler
{
    public static byte[] CompileHlsl(ShaderStage stage, string hlsl, string entryPoint, bool debug = false)
    {
        IDxcUtils utils = DxcCreateInstance<IDxcUtils>(CLSID_DxcUtils);
        
        IDxcCompiler3 compiler = DxcCreateInstance<IDxcCompiler3>(CLSID_DxcCompiler);

        string profile = stage switch
        {
            ShaderStage.Vertex => "vs_6_0",
            ShaderStage.Pixel => "ps_6_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        List<string> args = ["-spirv", "-T", profile, "-E", entryPoint];
        
        if (debug)
            args.Add("-Od");

        IDxcIncludeHandler handler = utils.CreateDefaultIncludeHandler();
        
        IDxcResult result = compiler.Compile(hlsl, args.ToArray(), handler);

        result.GetStatus().CheckError();

        IDxcBlob blob = result.GetResult();
        byte[] bytes = blob.AsBytes();

        blob.Release();
        result.Release();
        compiler.Release();
        utils.Release();

        return bytes;
    }
}