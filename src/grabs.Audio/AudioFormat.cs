namespace grabs.Audio;

public struct AudioFormat
{
    public DataType DataType;

    public uint SampleRate;

    public Channels Channels;

    public AudioFormat(DataType dataType, uint sampleRate, Channels channels)
    {
        DataType = dataType;
        SampleRate = sampleRate;
        Channels = channels;
    }
}