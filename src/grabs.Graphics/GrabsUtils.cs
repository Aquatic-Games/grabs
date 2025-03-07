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