namespace grabs.Graphics;

/// <summary>
/// A region of memory on a GPU device that can be used for rendering.
/// </summary>
public abstract class Buffer : MappableResource, IDisposable
{
    public readonly BufferInfo Info;

    protected Buffer(ref readonly BufferInfo info)
    {
        Info = info;
    }
    
    /// <summary>
    /// Release this buffer from memory.
    /// </summary>
    public abstract void Dispose();
}