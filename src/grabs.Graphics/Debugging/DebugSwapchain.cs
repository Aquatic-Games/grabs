using grabs.Core;

namespace grabs.Graphics.Debugging;

internal sealed class DebugSwapchain(Swapchain swapchain) : Swapchain
{
    private bool _readyForNextTexture = true;
    private Dictionary<Texture, DebugTexture> _swapchainTextures = [];
    
    public override bool IsDisposed
    {
        get => swapchain.IsDisposed;
        protected set => throw new NotImplementedException();
    }

    public override Format BufferFormat => swapchain.BufferFormat;

    public override Texture GetNextTexture()
    {
        if (!_readyForNextTexture)
            throw new ValidationException("Cannot get next texture. You must call Present() before getting the next texture.");

        _readyForNextTexture = false;

        Texture texture = swapchain.GetNextTexture();

        if (!_swapchainTextures.TryGetValue(texture, out DebugTexture debugTexture))
        {
            GrabsLog.Log("Creating and cacheing debug texture.");
            debugTexture = new DebugTexture(texture, BufferFormat);
            _swapchainTextures.Add(texture, debugTexture);
        }

        return debugTexture;
    }
    
    public override void Present()
    {
        if (_readyForNextTexture)
            throw new ValidationException("Cannot present without first calling GetNextTexture()");
        
        swapchain.Present();

        _readyForNextTexture = true;
    }
    
    public override void Dispose()
    {
        swapchain.Dispose();
    }
}