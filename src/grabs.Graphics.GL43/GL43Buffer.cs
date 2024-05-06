using System;
using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public sealed class GL43Buffer : Buffer
{
    private GL _gl;

    public readonly uint Buffer;
    public BufferTargetARB Target;
    
    public unsafe GL43Buffer(GL gl, in BufferDescription description, void* pData) : base(description)
    {
        _gl = gl;

        Target = description.Type switch
        {
            BufferType.Vertex => BufferTargetARB.ArrayBuffer,
            BufferType.Index => BufferTargetARB.ElementArrayBuffer,
            BufferType.Constant => BufferTargetARB.UniformBuffer,
            _ => throw new ArgumentOutOfRangeException()
        };
        
        Buffer = _gl.GenBuffer();
        _gl.BindBuffer(Target, Buffer);
        _gl.BufferData(Target, (nuint) description.SizeInBytes, pData,
            description.Dynamic ? GLEnum.DynamicDraw : GLEnum.StaticDraw);
    }
    
    public override void Dispose()
    {
        _gl.DeleteBuffer(Buffer);
    }
}