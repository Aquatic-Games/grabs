namespace grabs.Graphics;

/// <summary>
/// Various modes used for presentation.
/// </summary>
public enum PresentMode
{
    /// <summary>
    /// Present as soon as possible. This is equivalent to VSync off.
    /// </summary>
    Immediate,
    
    /// <summary>
    /// Wait for vertical sync before presenting. This is equivalent to VSync on.
    /// </summary>
    Fifo,
    
    //Mailbox,
}