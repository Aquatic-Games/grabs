using System.Runtime.CompilerServices;
using grabs.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.CLSID;
using static TerraFX.Interop.Windows.Windows;

namespace grabs.ShaderCompiler;

public static unsafe class Compiler
{
    public static byte[] CompileHlsl(ShaderStage stage, string hlsl, string entryPoint, bool debug = false)
    {
        Guid dxcUtils = CLSID_DxcUtils;
        Guid dxcCompiler = CLSID_DxcCompiler;

        IDxcUtils* utils;
        if (DxcCreateInstance(&dxcUtils, __uuidof<IDxcUtils>(), (void**) &utils).FAILED)
            throw new Exception("Failed to create DXC utils.");
        
        IDxcCompiler3* compiler;
        if (DxcCreateInstance(&dxcCompiler, __uuidof<IDxcCompiler3>(), (void**) &compiler).FAILED)
            throw new Exception("Failed to create DXC compiler.");

        string profile = stage switch
        {
            ShaderStage.Vertex => "vs_6_0",
            ShaderStage.Pixel => "ps_6_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        List<string> args = ["-spirv"];
        
        if (debug)
            args.Add("-Od");

        using PinnedString pHlsl = new PinnedString(hlsl);
        using WidePinnedString pEntryPoint = entryPoint;
        using WidePinnedString pProfile = profile;
        using WidePinnedStringArray pArgs = new WidePinnedStringArray(args);

        IDxcCompilerArgs* compilerArgs;
        utils->BuildArguments(null, pEntryPoint, pProfile, null, 0, null, 0, &compilerArgs);

        DxcBuffer buffer = new DxcBuffer()
        {
            Ptr = pHlsl,
            Size = (nuint) hlsl.Length * sizeof(byte),
            Encoding = 0
        };

        IDxcResult* result;
        if (compiler->Compile(&buffer, compilerArgs->GetArguments(), compilerArgs->GetCount(), null,
                __uuidof<IDxcResult>(), (void**) &result).FAILED)
        {
            throw new Exception("Failed to compile.");
        }

        HRESULT status;
        if (result->GetStatus(&status).FAILED)
            throw new Exception("Failed to get compile status.");

        if (status.FAILED)
        {
            IDxcBlobEncoding* errorBlob;
            result->GetErrorBuffer(&errorBlob);

            throw new Exception(new string((sbyte*) errorBlob->GetBufferPointer()));
        }

        IDxcBlob* outResult;
        result->GetResult(&outResult);

        byte[] bytes = new byte[outResult->GetBufferSize()];

        fixed (byte* pBytes = bytes)
            Unsafe.CopyBlock(pBytes, outResult->GetBufferPointer(), (uint) outResult->GetBufferSize());

        outResult->Release();
        result->Release();
        compilerArgs->Release();
        compiler->Release();
        utils->Release();

        return bytes;
    }
}