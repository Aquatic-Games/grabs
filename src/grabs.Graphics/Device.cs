namespace grabs.Graphics;

/// <summary>
/// Represents a logical device that can have commands issued to it.
/// </summary>
public abstract class Device : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="Device"/> is disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Create a <see cref="Swapchain"/>.
    /// </summary>
    /// <param name="info">The <see cref="SwapchainInfo"/> used to describe the swapchain.</param>
    /// <returns>The created <see cref="Swapchain"/>.</returns>
    public abstract Swapchain CreateSwapchain(in SwapchainInfo info);

    /// <summary>
    /// Create a <see cref="CommandList"/>.
    /// </summary>
    /// <returns>The created <see cref="CommandList"/>.</returns>
    public abstract CommandList CreateCommandList();

    /// <summary>
    /// Execute the commands in the command list.
    /// </summary>
    /// <param name="cl">The <see cref="CommandList"/> to execute.</param>
    public abstract void ExecuteCommandList(CommandList cl);

    /// <summary>
    /// Dispose of this <see cref="Device"/>.
    /// </summary>
    public abstract void Dispose();
}