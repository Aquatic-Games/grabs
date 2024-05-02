using System;
using System.Runtime.CompilerServices;
using System.Text;
using grabs.Graphics;
using TerraFX.Interop.DirectX;
using TerraFX.Interop.Windows;
using static TerraFX.Interop.DirectX.DXC;
using static TerraFX.Interop.DirectX.DirectX;
using static TerraFX.Interop.Windows.CLSID;
using static TerraFX.Interop.Windows.Windows;

namespace grabs.ShaderCompiler.DXC;

public static unsafe class Compiler
{
    public static byte[] CompileToSpirV(string code, string entryPoint, ShaderStage stage, bool debug = false)
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

        IDxcCompiler* compiler;
        if ((result = DxcCreateInstance(&dxcCompilerGuid, __uuidof<IDxcCompiler>(), (void**) &compiler)).FAILED)
            throw new Exception($"Failed to create DXC compiler: {result}");

        string profile = stage switch
        {
            ShaderStage.Vertex => "vs_6_0",
            ShaderStage.Pixel => "ps_6_0",
            ShaderStage.Compute => "cs_6_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        using PinnedS
        string[] args = ["-spirv", debug ? "-O0" : ""];
        
        IDxcOperationResult* opResult;
        if ((result = compiler->Compile((IDxcBlob*) blobEncoding, null, pEntryPoint, pTargetProfile, )).Failure)
            throw new Exception($"Failed to compile shader: {result.Description}");

        if ((result = opResult.GetStatus(out Result status)).Failure)
            throw new Exception($"Failed to get result status: {result.Description}");
        if (status.Failure)
            throw new Exception($"Failed to compile shader: {new string((sbyte*) opResult.GetErrorBuffer().BufferPointer)}");

        IDxcBlob blob;
        if ((result = opResult.GetResult(out blob)).Failure)
            throw new Exception($"Failed to get result: {result}");

        byte[] bytes = new byte[blob.BufferSize];
        fixed (byte* pBytes = bytes)
            Unsafe.CopyBlock(pBytes, (void*) blob.BufferPointer, (uint) blob.BufferSize);
        
        blob.Dispose();
        opResult.Dispose();
        compiler.Dispose();
        blobEncoding.Dispose();
        utils.Dispose();

        return bytes;
    }
}