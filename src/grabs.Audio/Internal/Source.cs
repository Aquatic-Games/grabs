using System.Collections.Generic;

namespace grabs.Audio.Internal;

internal struct Source
{
    public Queue<ulong> QueuedBuffers;

    public bool Playing;

    public double Speed;
    
    public ulong Position;
    public double FinePosition;
}