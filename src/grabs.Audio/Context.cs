using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using grabs.Audio.Internal;
using Buffer = grabs.Audio.Internal.Buffer;

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

    public unsafe AudioBuffer CreateBuffer<T>(in AudioFormat format, in ReadOnlySpan<T> data) where T : unmanaged
    {
        if (_numBuffers + 1 >= (ulong) _buffers.Length)
            Array.Resize(ref _buffers, _buffers.Length << 1);

        uint dataLength = (uint) (data.Length * sizeof(T));
        byte[] byteData = new byte[dataLength];
        
        fixed (void* pByteData = byteData)
        fixed (void* pData = data)
            Unsafe.CopyBlock(pByteData, pData, dataLength);

        ulong bufferIndex = _numBuffers++;
        _buffers[bufferIndex] = new Buffer(byteData, format);

        return new AudioBuffer(this, bufferIndex);
    }

    public AudioSource CreateSource()
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

    internal void SourcePlay(ulong sourceId)
    {
        _sources[sourceId].Playing = true;
    }

    internal void MixIntoBufferStereoF32(Span<float> buffer)
    {
        for (int i = 0; i < buffer.Length; i += 2)
        {
            buffer[i + 0] = 0;
            buffer[i + 1] = 0;
            
            for (ulong s = 0; s < _numSources; s++)
            {
                ref Source source = ref _sources[s];
                
                if (!source.Playing)
                    continue;

                ulong bufferId = source.QueuedBuffers.Peek();
                ref Buffer buf = ref _buffers[bufferId];

                ref AudioFormat format = ref buf.Format;

                ulong bytePosition = source.Position * buf.ByteAlign * buf.Channels;

                float sampleL = GetSample(buf.Data, bytePosition, format.DataType);
                float sampleR = GetSample(buf.Data, bytePosition + buf.ByteAlign, format.DataType);

                buffer[i + 0] = sampleL;
                buffer[i + 1] = sampleR;

                source.Position++;
            }
        }
    }

    private unsafe float GetSample(byte[] data, ulong index, DataType type)
    {
        switch (type)
        {
            case DataType.U8:
                throw new NotImplementedException();
            case DataType.I16:
                return (short) (data[index + 0] | data[index + 1] << 8) / (float) short.MaxValue;
            case DataType.I32:
                throw new NotImplementedException();
            case DataType.F32:
            {
                int result = data[index + 0] | (data[index + 1] << 8) | (data[index + 2] << 16) | (data[index + 3] << 24);
                return *(float*) &result;
            }
            default:
                throw new ArgumentOutOfRangeException(nameof(type), type, null);
        }
    }
}