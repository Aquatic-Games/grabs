﻿using System;
using grabs.ShaderCompiler.Spirv;
using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public sealed class GL43ShaderModule : ShaderModule
{
    private readonly GL _gl;
    
    public readonly uint Shader;

    public readonly DescriptorRemappings DescriptorRemappings;
    
    public GL43ShaderModule(GL gl, ShaderStage stage, byte[] spirv, string entryPoint, SpecializationConstant[] constants) 
        : base(stage)
    {
        _gl = gl;

        ShaderType type = stage switch
        {
            ShaderStage.Vertex => ShaderType.VertexShader,
            ShaderStage.Pixel => ShaderType.FragmentShader,
            ShaderStage.Compute => ShaderType.ComputeShader,
            _ => throw new ArgumentOutOfRangeException(nameof(stage), stage, null)
        };

        Shader = _gl.CreateShader(type);

        string source = SpirvCompiler.TranspileSpirv(stage, ShaderLanguage.Glsl430, spirv, entryPoint,
            out DescriptorRemappings, constants);
        
        _gl.ShaderSource(Shader, source);
        _gl.CompileShader(Shader);

        _gl.GetShader(Shader, ShaderParameterName.CompileStatus, out int status);
        if (status != (int) GLEnum.True)
            throw new Exception($"Failed to compile {stage} shader: {_gl.GetShaderInfoLog(Shader)}");
    }
    
    public override void Dispose()
    {
        _gl.DeleteShader(Shader);
    }
}