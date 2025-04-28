namespace grabs.Graphics;

public abstract class CommandList : IDisposable
{
    /// <summary>
    /// Gets if this <see cref="CommandList"/> has been disposed.
    /// </summary>
    public abstract bool IsDisposed { get; protected set; }

    /// <summary>
    /// Begin the command list. You <b>must</b> call this before issuing any commands.
    /// </summary>
    /// <remarks>This will reset the command list.</remarks>
    public abstract void Begin();

    /// <summary>
    /// End the command list. You <b>must</b> call this before executing it.
    /// </summary>
    public abstract void End();

    /// <summary>
    /// Dispose of this <see cref="CommandList"/>.
    /// </summary>
    public abstract void Dispose();
}