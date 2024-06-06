using System;

namespace grabs.Audio;

public struct AudioBuffer : IDisposable
{ 
    private Context _context;
    
    internal ulong Id;

    internal AudioBuffer(Context context, ulong id)
    {
        _context = context;
        Id = id;
    }
    
    public void Dispose()
    {
        
    }
}