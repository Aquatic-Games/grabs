using System.Runtime.CompilerServices;

namespace grabs.Graphics;

/// <summary>
/// Contains various utility functions useful during development.
/// </summary>
public static class GrabsUtils
{
    /// <summary>
    /// Check if the given format is an SRGB format.
    /// </summary>
    /// <param name="format">The format to check.</param>
    /// <returns>True, if the given format is SRGB.</returns>
    public static bool IsSrgb(this Format format)
    {
        switch (format)
        {
            case Format.R8G8B8A8_UNorm_SRGB:
            case Format.B8G8R8A8_UNorm_SRGB:
            case Format.BC1_UNorm_SRGB:
            case Format.BC2_UNorm_SRGB:
            case Format.BC3_UNorm_SRGB:
            case Format.BC7_UNorm_SRGB:
                return true;
            default:
                return false;
        }
    }

    public static uint BitsPerPixel(this Format format)
    {
        switch (format)
        {
            case Format.R8_UNorm:
            case Format.R8_UInt:
            case Format.R8_SNorm:
            case Format.R8_SInt:
            case Format.A8_UNorm:
                return 8;
            
            case Format.R8G8_UNorm:
            case Format.R8G8_UInt:
            case Format.R8G8_SNorm:
            case Format.R8G8_SInt:
            case Format.R16_Float:
            case Format.D16_UNorm:
            case Format.R16_UNorm:
            case Format.R16_UInt:
            case Format.R16_SNorm:
            case Format.R16_SInt:
            case Format.B5G6R5_UNorm:
            case Format.B5G5R5A1_UNorm:
                return 16;
            
            case Format.R8G8B8A8_UNorm:
            case Format.R8G8B8A8_UNorm_SRGB:
            case Format.R8G8B8A8_UInt:
            case Format.R8G8B8A8_SNorm:
            case Format.R8G8B8A8_SInt:
            case Format.B8G8R8A8_UNorm:
            case Format.B8G8R8A8_UNorm_SRGB:
            case Format.R16G16_Float:
            case Format.R16G16_UNorm:
            case Format.R16G16_UInt:
            case Format.R16G16_SNorm:
            case Format.R16G16_SInt:
            case Format.R32_Float:
            case Format.R32_UInt:
            case Format.R32_SInt:
            case Format.D24_UNorm_S8_UInt:
            case Format.D32_Float:
                return 32;
            
            case Format.R16G16B16A16_Float:
            case Format.R16G16B16A16_UNorm:
            case Format.R16G16B16A16_UInt:
            case Format.R16G16B16A16_SNorm:
            case Format.R16G16B16A16_SInt:
            case Format.R32G32_Float:
            case Format.R32G32_UInt:
            case Format.R32G32_SInt:
                return 64;
            
            case Format.R32G32B32_Float:
            case Format.R32G32B32_UInt:
            case Format.R32G32B32_SInt:
                return 96;
            
            case Format.R32G32B32A32_Float:
            case Format.R32G32B32A32_UInt:
            case Format.R32G32B32A32_SInt:
                return 128;
            
            case Format.BC1_UNorm:
            case Format.BC1_UNorm_SRGB:
            case Format.BC4_UNorm:
            case Format.BC4_SNorm:
                return 4;
                
            case Format.BC2_UNorm:
            case Format.BC2_UNorm_SRGB:
            case Format.BC3_UNorm:
            case Format.BC3_UNorm_SRGB:
            case Format.BC5_UNorm:
            case Format.BC5_SNorm:
            case Format.BC6H_UF16:
            case Format.BC6H_SF16:
            case Format.BC7_UNorm:
            case Format.BC7_UNorm_SRGB:
                return 8;
            
            case Format.Unknown:
            default:
                throw new ArgumentOutOfRangeException(nameof(format), format, null);
        }
    }

    public static uint BytesPerPixel(this Format format)
        => format.BitsPerPixel() / 8;

    public static unsafe void CopyData<T>(nint dataPtr, T data) where T : unmanaged
    {
        Unsafe.CopyBlock((void*) dataPtr, Unsafe.AsPointer(ref data), (uint) sizeof(T));
    }

    /// <summary>
    /// Copy managed data into an unmanaged region of memory.
    /// </summary>
    /// <param name="dataPtr">The pointer to the unmanaged data.</param>
    /// <param name="data">The managed data to copy.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    public static unsafe void CopyData<T>(nint dataPtr, in ReadOnlySpan<T> data) where T : unmanaged
    {
        uint dataSize = (uint) (data.Length * sizeof(T));
        
        fixed (void* pData = data)
            Unsafe.CopyBlock((void*) dataPtr, pData, dataSize);
    }

    /// <summary>
    /// Copy managed data into an unmanaged region of memory.
    /// </summary>
    /// <param name="dataPtr">The pointer to the unmanaged data.</param>
    /// <param name="data">The managed data to copy.</param>
    /// <typeparam name="T">Any unmanaged type.</typeparam>
    public static void CopyData<T>(nint dataPtr, T[] data) where T : unmanaged
        => CopyData<T>(dataPtr, data.AsSpan());
}