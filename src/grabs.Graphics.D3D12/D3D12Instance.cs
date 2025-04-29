namespace grabs.Graphics.D3D12;

internal sealed class D3D12Instance : Instance
{
    public override bool IsDisposed { get; protected set; }

    public override string BackendName => D3D12Backend.Name;
    
    public override Adapter[] EnumerateAdapters()
    {
        throw new NotImplementedException();
    }
    
    public override Surface CreateSurface(in SurfaceInfo info)
    {
        throw new NotImplementedException();
    }
    
    public override Device CreateDevice(Surface surface, Adapter? adapter = null)
    {
        throw new NotImplementedException();
    }
    
    public override void Dispose()
    {
        if (IsDisposed)
            return;
        IsDisposed = true;
    }
}