namespace grabs.Graphics;

/// <summary>
/// Defines various presentation modes, used 
/// </summary>
public enum PresentMode
{
    /// <summary>
    /// Immediately presents to the swapchain when the image is available. This is equivalent to VSync off.
    /// </summary>
    Immediate,
    
    /// <summary>
    /// Waits for vertical sync before getting the next swapchain texture. This is equivalent to VSync enabled.
    /// </summary>
    Fifo,
    
    /// <summary>
    /// Waits for vertical sync when possible, but does not wait if the frame cannot be processed fast enough.
    /// This is equivalent to adaptive VSync.
    /// </summary>
    FifoRelaxed,
    
    /// <summary>
    /// Waits for vertical sync, but does wait to get the next swapchain texture. This can improve input lag.
    /// </summary>
    Mailbox
}