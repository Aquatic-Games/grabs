using System;

namespace grabs.Audio.Internal;

internal struct Buffer
{
    public byte[] Data;
    public AudioFormat Format;

    public ulong ByteAlign;
    public ulong Channels;

    public Buffer(byte[] data, AudioFormat format)
    {
        Data = data;
        Format = format;

        ByteAlign = (ulong) format.DataType.Bytes();

        Channels = format.Channels switch
        {
            Audio.Channels.Mono => 1,
            Audio.Channels.Stereo => 2,
            _ => throw new ArgumentOutOfRangeException()
        };
    }
}