namespace grabs.Graphics.Debugging;

internal sealed class DebugInstance(Instance instance) : Instance
{
    public override bool IsDisposed
    {
        get => instance.IsDisposed;
        protected set => throw new NotImplementedException();
    }

    public override string BackendName => instance.BackendName;

    public override Adapter[] EnumerateAdapters()
        => instance.EnumerateAdapters();

    public override Surface CreateSurface(in SurfaceInfo info)
        => instance.CreateSurface(in info);

    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
        => new DebugDevice(instance.CreateDevice(surface, adapter));
    
    public override void Dispose()
    {
        instance.Dispose();
    }
}