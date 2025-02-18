namespace grabs.Graphics;

public readonly struct MappedData
{
    public readonly nint DataPointer;

    public MappedData(IntPtr dataPointer)
    {
        DataPointer = dataPointer;
    }
}