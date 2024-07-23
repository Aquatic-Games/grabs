using System;
using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public static class GLUtils
{
    public static DepthFunction ComparisonFunctionToGL(ComparisonFunction comparison)
    {
        return comparison switch
        {
            ComparisonFunction.Never => DepthFunction.Never,
            ComparisonFunction.Less => DepthFunction.Less,
            ComparisonFunction.Equal => DepthFunction.Equal,
            ComparisonFunction.LessEqual => DepthFunction.Lequal,
            ComparisonFunction.Greater => DepthFunction.Greater,
            ComparisonFunction.NotEqual => DepthFunction.Notequal,
            ComparisonFunction.GreaterEqual => DepthFunction.Gequal,
            ComparisonFunction.Always => DepthFunction.Always,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
    
    public static BlendingFactor BlendFactorToGL(BlendFactor factor)
    {
        return factor switch
        {
            BlendFactor.Zero => BlendingFactor.Zero,
            BlendFactor.One => BlendingFactor.One,
            BlendFactor.SrcColor => BlendingFactor.SrcColor,
            BlendFactor.OneMinusSrcColor => BlendingFactor.OneMinusSrcColor,
            BlendFactor.DestColor => BlendingFactor.DstColor,
            BlendFactor.OneMinusDestColor => BlendingFactor.OneMinusDstColor,
            BlendFactor.SrcAlpha => BlendingFactor.SrcAlpha,
            BlendFactor.OneMinusSrcAlpha => BlendingFactor.OneMinusSrcAlpha,
            BlendFactor.DestAlpha => BlendingFactor.DstAlpha,
            BlendFactor.OneMinusDestAlpha => BlendingFactor.OneMinusDstAlpha,
            BlendFactor.ConstantColor => BlendingFactor.ConstantColor,
            BlendFactor.OneMinusConstantColor => BlendingFactor.OneMinusConstantColor,
            BlendFactor.SrcAlphaSaturate => BlendingFactor.SrcAlphaSaturate,
            BlendFactor.Src1Color => BlendingFactor.Src1Color,
            BlendFactor.OneMinusSrc1Color => BlendingFactor.OneMinusSrc1Color,
            BlendFactor.Src1Alpha => BlendingFactor.Src1Alpha,
            BlendFactor.OneMinusSrc1Alpha => BlendingFactor.OneMinusSrc1Alpha,
            _ => throw new ArgumentOutOfRangeException(nameof(factor), factor, null)
        };
    }

    public static BlendEquationModeEXT BlendOperationToGL(BlendOperation operation)
    {
        return operation switch
        {
            BlendOperation.Add => BlendEquationModeEXT.FuncAdd,
            BlendOperation.Subtract => BlendEquationModeEXT.FuncSubtract,
            BlendOperation.ReverseSubtract => BlendEquationModeEXT.FuncReverseSubtract,
            BlendOperation.Min => BlendEquationModeEXT.Min,
            BlendOperation.Max => BlendEquationModeEXT.Max,
            _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
        };
    }

    public static TextureWrapMode TextureAddressToGL(TextureAddress address)
    {
        return address switch
        {
            TextureAddress.RepeatWrap => TextureWrapMode.Repeat,
            TextureAddress.RepeatWrapMirrored => TextureWrapMode.MirroredRepeat,
            TextureAddress.ClampToEdge => TextureWrapMode.ClampToEdge,
            TextureAddress.ClampToBorder => TextureWrapMode.ClampToBorder,
            _ => throw new ArgumentOutOfRangeException(nameof(address), address, null)
        };
    }
}