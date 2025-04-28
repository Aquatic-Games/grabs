namespace grabs.Graphics.Debugging;

internal sealed class DebugSwapchain(Swapchain swapchain) : Swapchain
{
    private bool _readyForNextTexture = true;
    
    public override bool IsDisposed
    {
        get => swapchain.IsDisposed;
        protected set => throw new NotImplementedException();
    }
    
    public override Texture GetNextTexture()
    {
        if (!_readyForNextTexture)
            throw new ValidationException("Cannot get next texture. You must call Present() before getting the next texture.");

        _readyForNextTexture = false;

        return swapchain.GetNextTexture();
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