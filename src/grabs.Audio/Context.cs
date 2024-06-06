using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using grabs.Audio.Internal;
using Buffer = grabs.Audio.Internal.Buffer;

namespace grabs.Audio;

public sealed class Context : IDisposable
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

    public unsafe AudioBuffer CreateBuffer<T>(in AudioFormat format, in ReadOnlySpan<T> data) where T : unmanaged
    {
        if (_numBuffers + 1 >= (ulong) _buffers.Length)
            Array.Resize(ref _buffers, _buffers.Length << 1);

        uint dataLength = (uint) (data.Length * sizeof(T));
        byte* byteData = (byte*) NativeMemory.Alloc(dataLength);
        
        fixed (void* pData = data)
            Unsafe.CopyBlock(byteData, pData, dataLength);

        ulong bufferIndex = _numBuffers++;
        _buffers[bufferIndex] = new Buffer(byteData, format);

        return new AudioBuffer(this, bufferIndex);
    }

    public unsafe AudioSource CreateSource()
    {
        if (_numSources + 1 >= (ulong) _buffers.Length)
            Array.Resize(ref _sources, _sources.Length << 1);

        ulong sourceIndex = _numSources++;
        _sources[sourceIndex] = new Source();

        return new AudioSource(this, sourceIndex);
    }

    internal void SubmitBufferToSource(ulong bufferId, ulong sourceId)
    {
        _sources[sourceId].QueuedBuffers.Enqueue(bufferId);
    }

    public void Dispose()
    {
        for (ulong i = 0; i < _numBuffers; i++)
            _buffers[i].Dispose();
    }
}