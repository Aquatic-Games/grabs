using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using grabs.Core;
using grabs.Graphics;
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

    public static byte[] CompileHlsl(ShaderStage stage, ShaderFormat format, string hlsl, string entryPoint,
        string? includeDir = null, bool debug = false)
    {
        switch (format)
        {
            case ShaderFormat.Unknown:
            {
                throw new NotSupportedException(
                    "Cannot compile for an unknown shader format. Shaders must be precompiled.");
            }
            
            case ShaderFormat.Spirv:
            case ShaderFormat.Dxil:
                return CompileHlslWithDxc(stage, hlsl, entryPoint, includeDir, debug, format == ShaderFormat.Spirv);
            
            case ShaderFormat.Dxbc:
                return CompileHlslWithD3DCompiler(stage, hlsl, entryPoint, includeDir, debug);
            
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    public static ShaderModule CreateShaderModuleFromHlsl(this Device device, ShaderStage stage, string hlsl, string entryPoint,
        string? includeDir = null, bool debug = false)
    {
        byte[] code = CompileHlsl(stage, device.ShaderFormat, hlsl, entryPoint, includeDir, debug);
        return device.CreateShaderModule(stage, code, entryPoint);
    }

    private static byte[] CompileHlslWithDxc(ShaderStage stage, string hlsl, string entryPoint, string? includeDir,
        bool debug, bool spirv)
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

        List<string> args = [];
        
        if (spirv)
            args.Add("-spirv");

        if (includeDir != null)
        {
            args.Add("-I");
            args.Add(includeDir);
        }

        if (debug)
            args.Add("-Od");

        using Utf8String pHlsl = new Utf8String(hlsl);
        using DxcString pEntryPoint = entryPoint;
        using DxcString pProfile = profile;
        using DxcStringArray pArgs = new DxcStringArray(args);

        IDxcCompilerArgs* compilerArgs;
        if (utils->BuildArguments(null, pEntryPoint, pProfile, pArgs, pArgs.Length, null, 0, &compilerArgs).FAILED)
        {
            compiler->Release();
            utils->Release();
            
            throw new Exception("Failed to build arguments.");
        }

        IDxcIncludeHandler* includeHandler;
        if (utils->CreateDefaultIncludeHandler(&includeHandler).FAILED)
        {
            compiler->Release();
            utils->Release();

            throw new Exception("Failed to create default include handler.");
        }

        DxcBuffer buffer = new DxcBuffer()
        {
            Ptr = (void*) pHlsl.Handle,
            Size = (nuint) hlsl.Length * sizeof(byte),
            Encoding = 0
        };

        IDxcResult* result;
        if (compiler->Compile(&buffer, compilerArgs->GetArguments(), compilerArgs->GetCount(), includeHandler,
                __uuidof<IDxcResult>(), (void**) &result).FAILED)
        {
            includeHandler->Release();
            compilerArgs->Release();
            compiler->Release();
            utils->Release();
            
            throw new Exception("Failed to compile.");
        }

        HRESULT status;
        if (result->GetStatus(&status).FAILED)
        {
            result->Release();
            includeHandler->Release();
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
            includeHandler->Release();
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
        includeHandler->Release();
        compilerArgs->Release();
        compiler->Release();
        utils->Release();

        return bytes;
    }

    private static byte[] CompileHlslWithD3DCompiler(ShaderStage stage, string hlsl, string entryPoint,
        string? includeDir, bool debug)
    {
        string target = stage switch
        {
            ShaderStage.Vertex => "vs_5_0",
            ShaderStage.Pixel => "ps_5_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        using Utf8String pTarget = target;
        using Utf8String pHlsl = hlsl;
        using Utf8String pEntryPoint = entryPoint;

        ID3DBlob* resultBlob;
        ID3DBlob* errorBlob;

        if (D3DCompile((void*) pHlsl.Handle, (nuint) hlsl.Length, null, null, null, pEntryPoint, pTarget, 0, 0,
                &resultBlob, &errorBlob).FAILED)
        {
            throw new Exception($"Failed to compile shader: {new string((sbyte*) resultBlob->GetBufferPointer())}");
        }

        if (errorBlob != null)
            errorBlob->Release();

        byte[] bytes = new byte[resultBlob->GetBufferSize()];

        fixed (byte* pBytes = bytes)
            Unsafe.CopyBlock(pBytes, resultBlob->GetBufferPointer(), (uint) resultBlob->GetBufferSize());

        resultBlob->Release();
        
        return bytes;
    }
}