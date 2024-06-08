using System;

namespace grabs.Audio;

public struct AudioSource : IDisposable
{
    internal ulong Id;
    private Context _context;

    public double Speed
    {
        get => throw new NotImplementedException();
        set => _context.SourceSetSpeed(Id, value);
    }

    public float Volume
    {
        get => throw new NotImplementedException();
        set => _context.SourceSetVolume(Id, value);
    }

    public bool Looping
    {
        get => throw new NotImplementedException();
        set => _context.SourceSetLooping(Id, value);
    }

    public AudioSource(Context context, ulong id)
    {
        _context = context;
        Id = id;
    }

    public void SubmitBuffer(AudioBuffer buffer)
    {
        _context.SubmitBufferToSource(buffer.Id, Id);
    }

    public void Play()
    {
        _context.SourcePlay(Id);
    }

    public void Dispose()
    {
        
    }
}