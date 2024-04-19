using System;
using System.Runtime.CompilerServices;
using System.Text;
using grabs.Graphics;
using SharpGen.Runtime;
using Vortice.Dxc;
using static Vortice.Dxc.Dxc;

namespace grabs.ShaderCompiler.DXC;

public static unsafe class Compiler
{
    public static byte[] CompileToSpirV(string code, string entryPoint, ShaderStage stage, bool debug = false)
    {
        Result result;
        
        IDxcUtils utils;
        if ((result = DxcCreateInstance(CLSID_DxcUtils, out utils!)).Failure)
            throw new Exception($"Failed to create DXC utils: {result.Description}");

        IDxcBlobEncoding blobEncoding;
        fixed (byte* pCode = Encoding.UTF8.GetBytes(code))
        {
            if ((result = utils.CreateBlob((nint) pCode, code.Length, DXC_CP_UTF8, out blobEncoding)).Failure)
                throw new Exception($"Failed to create blob: {result.Description}");
        }

        IDxcCompiler compiler;
        if ((result = DxcCreateInstance(CLSID_DxcCompiler, out compiler!)).Failure)
            throw new Exception($"Failed to create DXC compiler: {result.Description}");

        string profile = stage switch
        {
            ShaderStage.Vertex => "vs_6_0",
            ShaderStage.Pixel => "ps_6_0",
            ShaderStage.Compute => "cs_6_0",
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };
        
        IDxcOperationResult opResult;
        if ((result = compiler.Compile(blobEncoding, null, entryPoint, profile, ["-spirv", debug ? "-O0" : ""], [], null, out opResult!)).Failure)
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