using System;

namespace grabs.Audio;

public struct AudioBuffer : IDisposable
{
    private ulong _id;
    private Context _context;

    internal AudioBuffer(ulong id, Context context)
    {
        _id = id;
        _context = context;
    }
    
    public void Dispose()
    {
        
    }
}