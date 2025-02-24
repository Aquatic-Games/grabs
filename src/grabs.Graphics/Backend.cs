namespace grabs.Graphics;

/// <summary>
/// Contains various backend APIs that can be used for rendering.
/// </summary>
[Flags]
public enum Backend
{
    /// <summary>
    /// Auto/private API.
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// Vulkan 1.3
    /// </summary>
    Vulkan = 1 << 0,
    
    /// <summary>
    /// DirectX 11.1
    /// </summary>
    D3D11 = 1 << 1
}