namespace grabs.Graphics;

public readonly struct MappedData
{
    public readonly nint DataPtr;

    public MappedData(IntPtr dataPtr)
    {
        DataPtr = dataPtr;
    }
}