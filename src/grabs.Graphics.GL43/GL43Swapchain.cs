namespace grabs.Graphics.GL43;

public sealed class GL43Swapchain : Swapchain
{
    private GL43Surface _surface;
    
    private PresentMode _presentMode;
    private int _swapInterval;

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

    public GL43Swapchain(GL43Surface surface, in SwapchainDescription description)
    {
        _surface = surface;

        Width = description.Width;
        Height = description.Height;
        
        PresentMode = description.PresentMode;
    }

    public override Texture GetSwapchainTexture()
    {
        return new GL43SwapchainTexture();
    }

    public override void Present()
    {
        _surface.PresentFunc(_swapInterval);
    }

    public override void Dispose() { }
}