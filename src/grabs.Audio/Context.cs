using System;

namespace grabs.Audio;

public sealed class Context
{
    private uint _sampleRate;

    private ulong _numBuffers;
    private Buffer[] _buffers;

    private ulong _numSources;
    private Source[] _sources;

    public float MasterVolume;
    
    public Context(uint sampleRate)
    {
        _sampleRate = sampleRate;

        _buffers = new Buffer[1];
        _sources = new Source[1];

        MasterVolume = 1.0f;
    }

    public AudioBuffer CreateBuffer()
    {
        if (_numBuffers + 1 >= (ulong) _buffers.Length)
            Array.Resize(ref _buffers, _buffers.Length << 1);

        ulong bufferIndex = _numBuffers++;
        _buffers[bufferIndex] = new Buffer();

        return new AudioBuffer(bufferIndex, this);
    }
}