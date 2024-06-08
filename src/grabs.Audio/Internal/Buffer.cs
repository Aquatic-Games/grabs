using System;

namespace grabs.Audio.Internal;

internal struct Buffer
{
    public byte[] Data;
    public AudioFormat Format;

    public ulong LengthInSamples;
    
    public ulong ByteAlign;
    public ulong Channels;

    public double SpeedCorrection;
}