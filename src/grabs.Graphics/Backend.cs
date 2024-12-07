namespace grabs.Graphics;

/// <summary>
/// Corresponds to a graphics API used for rendering.
/// </summary>
[Flags]
public enum Backend
{
    /// <summary>
    /// Unknown/Auto backend. In <see cref="Instance.Create()"/>, a backend will automatically be selected based on
    /// what the system supports. This value could also mean private API backends, and could be returned from
    /// <see cref="Instance.Backend"/>.
    /// </summary>
    Unknown = 0,
    
    /// <summary>
    /// Vulkan 1.3 backend.
    /// </summary>
    Vulkan = 1 << 0,
    
    /// <summary>
    /// Direct3D 11.1 backend.
    /// </summary>
    D3D11 = 1 << 1
}