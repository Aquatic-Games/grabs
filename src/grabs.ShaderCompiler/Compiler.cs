using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using grabs.Core;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.CLSID;
using static TerraFX.Interop.Windows.Windows;

namespace grabs.ShaderCompiler;

[SuppressMessage("Interoperability", "CA1416:Validate platform compatibility")]
public static unsafe class Compiler
{
    static Compiler()
    {
        ResolveLibrary += OnResolveLibrary;
    }

    private static IntPtr OnResolveLibrary(string libraryname, Assembly assembly, DllImportSearchPath? searchpath)
    {
        return NativeLibrary.Load(libraryname, assembly, DllImportSearchPath.AssemblyDirectory);
    }

    public static byte[] CompileHlsl(ShaderStage stage, string hlsl, string entryPoint, bool debug = false)
    {
        Guid dxcUtils = CLSID_DxcUtils;
        Guid dxcCompiler = CLSID_DxcCompiler;

        IDxcUtils* utils;
        if (DxcCreateInstance(&dxcUtils, __uuidof<IDxcUtils>(), (void**) &utils).FAILED)
            throw new Exception("Failed to create DXC utils.");
        
        IDxcCompiler3* compiler;
        if (DxcCreateInstance(&dxcCompiler, __uuidof<IDxcCompiler3>(), (void**) &compiler).FAILED)
        {
            utils->Release();
            
            throw new Exception("Failed to create DXC compiler.");
        }

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
        if (utils->BuildArguments(null, pEntryPoint, pProfile, pArgs, pArgs.Length, null, 0, &compilerArgs).FAILED)
        {
            compiler->Release();
            utils->Release();
            
            throw new Exception("Failed to build arguments.");
        }

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
            compilerArgs->Release();
            compiler->Release();
            utils->Release();
            
            throw new Exception("Failed to compile.");
        }

        HRESULT status;
        if (result->GetStatus(&status).FAILED)
        {
            result->Release();
            compilerArgs->Release();
            compiler->Release();
            utils->Release();
            
            throw new Exception("Failed to get compile status.");
        }

        if (status.FAILED)
        {
            IDxcBlobEncoding* errorBlob;
            result->GetErrorBuffer(&errorBlob);
            string error = new string((sbyte*) errorBlob->GetBufferPointer());

            errorBlob->Release();
            result->Release();
            compilerArgs->Release();
            compiler->Release();
            utils->Release();

            throw new CompilationException(stage, error);
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