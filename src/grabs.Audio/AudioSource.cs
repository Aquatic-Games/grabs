using System;

namespace grabs.Audio;

public struct AudioSource : IDisposable
{
    private ulong _id;
    private Context _context;

    public AudioSource(ulong id, Context context)
    {
        _id = id;
        _context = context;
    }

    public void Dispose()
    {
        
    }
}