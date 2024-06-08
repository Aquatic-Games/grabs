using System;
using System.IO;

namespace grabs.Audio.Stream;

public class Wav : AudioStream
{
    private readonly BinaryReader _reader;
    
    private readonly long _dataStartPoint;
    private readonly int _dataLength;
    
    public override AudioFormat Format { get; }

    public Wav(string path)
    {
        const uint riff = 0x46464952;
        const uint wave = 0x45564157;
        const uint fmt  = 0x20746D66;
        const uint data = 0x61746164;
        
        _reader = new BinaryReader(File.OpenRead(path));

        if (_reader.ReadUInt32() != riff)
            throw new Exception("Given file is not a wav file.");

        _reader.ReadUInt32(); // File size

        if (_reader.ReadUInt32() != wave)
            throw new Exception("Given file is not a valid wav file.");

        while (true)
        {
            uint headerType = _reader.ReadUInt32();
            uint headerSize = _reader.ReadUInt32();
            
            switch (headerType)
            {
                case fmt:
                {
                    if (headerSize != 16)
                        throw new Exception("Cannot read wav files with unsupported format types.");

                    ushort type = _reader.ReadUInt16();
                    ushort channels = _reader.ReadUInt16();
                    uint sampleRate = _reader.ReadUInt32();

                    _reader.ReadUInt32();
                    _reader.ReadUInt16();

                    ushort bitsPerSample = _reader.ReadUInt16();

                    DataType dataType = (type, bitsPerSample) switch
                    {
                        (1, 8) => DataType.U8,
                        (1, 16) => DataType.I16,
                        (1, 32) => DataType.I32,
                        (3, 32) => DataType.F32,
                        _ => throw new NotSupportedException($"Data type not supported. (PCM type: {type}, Bits: {bitsPerSample})")
                    };

                    Channels channelsType = channels switch
                    {
                        1 => Channels.Mono,
                        2 => Channels.Stereo,
                        _ => throw new NotSupportedException($"Number of channels ({channels}) not supported.")
                    };

                    Format = new AudioFormat(dataType, sampleRate, channelsType);
                    
                    break;
                }
                
                case data:
                    _dataLength = (int) headerSize;
                    _dataStartPoint = _reader.BaseStream.Position;

                    return;
                
                default:
                    _reader.BaseStream.Position += headerSize;
                    break;
            }
        }
    }
    
    public override byte[] GetPCM()
    {
        _reader.BaseStream.Position = _dataStartPoint;
        byte[] data = _reader.ReadBytes(_dataLength);
        _reader.BaseStream.Position = _dataStartPoint;

        return data;
    }

    public override void Dispose()
    {
        _reader.Dispose();
    }
}