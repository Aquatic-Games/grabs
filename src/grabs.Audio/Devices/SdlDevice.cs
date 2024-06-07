using System;
using Silk.NET.SDL;

namespace grabs.Audio.Devices;

public class SdlDevice : AudioDevice
{
    private Sdl _sdl;
    private uint _deviceId;

    public unsafe SdlDevice(uint sampleRate, byte channels, uint periodSize = 512) : base(sampleRate)
    {
        _sdl = Sdl.GetApi();

        if (_sdl.Init(Sdl.InitAudio) < 0)
            throw new Exception("Failed to intialize SDL.");

        AudioSpec spec = new AudioSpec()
        {
            Freq = (int) sampleRate,
            Channels = channels,
            Format = Sdl.AudioF32,
            Samples = (ushort) periodSize,
            Callback = new PfnAudioCallback(AudioCallback)
        };

        _deviceId = _sdl.OpenAudioDevice((byte*) null, 0, &spec, null, 0);
        if (_deviceId == 0)
            throw new Exception("Failed to open audio device.");
        
        _sdl.PauseAudioDevice(_deviceId, 0);
    }

    private unsafe void AudioCallback(void* _, byte* buffer, int length)
    {
        GetBuffer(new Span<byte>(buffer, length));
    }

    public override void Dispose()
    {
        _sdl.CloseAudioDevice(_deviceId);
        _sdl.Dispose();
    }
}