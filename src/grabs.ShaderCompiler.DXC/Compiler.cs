using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using grabs.Core;
using grabs.Graphics;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DXC;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.CLSID;
using static TerraFX.Interop.Windows.Windows;

namespace grabs.ShaderCompiler.DXC;

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

    public static byte[] CompileToSpirV(string code, string entryPoint, ShaderStage stage, bool debug = false, string[] includeDirectories = null)
    {
        HRESULT result;
        
        Guid dxcUtilsGuid = CLSID_DxcUtils;
        Guid dxcCompilerGuid = CLSID_DxcCompiler;
        
        IDxcUtils* utils;
        if ((result = DxcCreateInstance(&dxcUtilsGuid, __uuidof<IDxcUtils>(), (void**) &utils)).FAILED)
            throw new Exception($"Failed to create DXC utils: {result}");

        IDxcBlobEncoding* blobEncoding;
        fixed (byte* pCode = Encoding.UTF8.GetBytes(code))
        {
            if ((result = utils->CreateBlob(pCode, (uint) code.Length, DXC_CP_UTF8, &blobEncoding)).FAILED)
                throw new Exception($"Failed to create blob: {result}");
        }

        IDxcCompiler3* compiler;
        if ((result = DxcCreateInstance(&dxcCompilerGuid, __uuidof<IDxcCompiler3>(), (void**) &compiler)).FAILED)
            throw new Exception($"Failed to create DXC compiler: {result}");

        string profile = stage switch
        {
            ShaderStage.Vertex => "vs_6_0",
            ShaderStage.Pixel => "ps_6_0",
            ShaderStage.Compute => "cs_6_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        using WidePinnedString pEntryPoint = new WidePinnedString(entryPoint);
        using WidePinnedString pTargetProfile = new WidePinnedString(profile);

        List<string> args = new List<string>()
        {
            "-spirv"
        };
        
        if (debug)
            args.Add("-Od");

        if (includeDirectories != null)
        {
            foreach (string directory in includeDirectories)
            {
                args.Add("-I");
                args.Add(directory);
            }
        }

        using WidePinnedStringArray pArgs = new WidePinnedStringArray(args.ToArray());

        IDxcCompilerArgs* compilerArgs;
        if ((result = utils->BuildArguments(null, pEntryPoint, pTargetProfile, pArgs, (uint) pArgs.Length, null, 0, &compilerArgs)).FAILED)
            throw new Exception($"Failed to build arguments: {result}");
            
        DxcBuffer buffer = new DxcBuffer()
        {
            Ptr = blobEncoding->GetBufferPointer(),
            Size = blobEncoding->GetBufferSize(),
            Encoding = 0
        };

        IDxcIncludeHandler* includeHandler;
        if ((result = utils->CreateDefaultIncludeHandler(&includeHandler)).FAILED)
            throw new Exception($"Failed to create include handler: {result}");
        
        IDxcOperationResult* opResult;
        if ((result = compiler->Compile(&buffer, compilerArgs->GetArguments(), compilerArgs->GetCount(), includeHandler, __uuidof<IDxcOperationResult>(), (void**) &opResult)).FAILED)
            throw new Exception($"Failed to compile shader: {result}");

        HRESULT status;
        if ((result = opResult->GetStatus(&status)).FAILED)
            throw new Exception($"Failed to get result status: {result}");
        if (status.FAILED)
        {
            IDxcBlobEncoding* errorBlob;
            opResult->GetErrorBuffer(&errorBlob);
            
            throw new Exception($"Failed to compile shader: {new string((sbyte*) errorBlob->GetBufferPointer())}");
        }

        IDxcBlob* blob;
        if ((result = opResult->GetResult(&blob)).FAILED)
            throw new Exception($"Failed to get result: {result}");

        byte[] bytes = new byte[blob->GetBufferSize()];
        fixed (byte* pBytes = bytes)
            Unsafe.CopyBlock(pBytes, blob->GetBufferPointer(), (uint) bytes.Length);
        
        blob->Release();
        opResult->Release();
        compilerArgs->Release();
        compiler->Release();
        blobEncoding->Release();
        utils->Release();

        return bytes;
    }
}