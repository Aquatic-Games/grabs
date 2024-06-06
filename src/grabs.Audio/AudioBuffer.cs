using System;

namespace grabs.Audio;

public struct AudioBuffer : IDisposable
{
    private ulong _id;
    private Context _context;
    
    public void Dispose()
    {
        
    }
}