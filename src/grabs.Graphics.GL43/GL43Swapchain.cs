using Silk.NET.OpenGL;

namespace grabs.Graphics.GL43;

public sealed class GL43Swapchain : Swapchain
{
    private GL _gl;
    
    private GL43Surface _surface;
    private GL43Texture _swapchainTexture;
    
    private PresentMode _presentMode;
    private int _swapInterval;

    private Format _swapchainFormat;
    
    public uint Width;
    public uint Height;
    
    public override PresentMode PresentMode
    {
        get => _presentMode;
        set
        {
            // Like DX, GL only supports Immediate and Vertical sync, so the code is just copy pasted from the DX layer.
            (_presentMode, _swapInterval) = value switch
            {
                PresentMode.Immediate => (PresentMode.Immediate, 0),
                PresentMode.VerticalSync => (PresentMode.VerticalSync, 1),
                PresentMode.AdaptiveSync => (PresentMode.VerticalSync, 1),
                _ => throw new ArgumentOutOfRangeException(nameof(value), value, null)
            };
        }
    }

    public GL43Swapchain(GL gl, GL43Surface surface, in SwapchainDescription description)
    {
        _gl = gl;
        _surface = surface;

        _swapchainFormat = description.Format;
        
        Width = description.Width;
        Height = description.Height;
        
        PresentMode = description.PresentMode;
    }

    public override unsafe Texture GetSwapchainTexture()
    {
        _swapchainTexture = new GL43Texture(_gl,
            TextureDescription.Texture2D(Width, Height, 1, _swapchainFormat,
                TextureUsage.Framebuffer | TextureUsage.ShaderResource), null);
        
        return _swapchainTexture;
    }

    public override void Present()
    {
        _surface.PresentFunc(_swapInterval);
    }

    public override void Dispose() { }
}