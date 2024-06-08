namespace grabs.Audio;

public struct BufferDescription
{
    public PcmType PcmType;
    public AudioFormat Format;

    public BufferDescription(PcmType pcmType, AudioFormat format)
    {
        PcmType = pcmType;
        Format = format;
    }
}