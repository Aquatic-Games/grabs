namespace grabs.Graphics.Debugging;

internal sealed class DebugBuffer : Buffer
{
    public override bool IsDisposed
    {
        get => Buffer.IsDisposed;
        protected set => throw new NotImplementedException();
    }

    public readonly Buffer Buffer;

    public DebugBuffer(Buffer buffer)
    {
        Buffer = buffer;
    }
    
    public override void Dispose()
    {
        Buffer.Dispose();
    }
}