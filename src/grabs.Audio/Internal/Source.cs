using System.Collections.Generic;

namespace grabs.Audio.Internal;

internal struct Source
{
    public Queue<ulong> QueuedBuffers;

    public bool Playing;

    public double Speed;
    public float Volume;
    public bool Looping;
    
    public ulong Position;
    public double FinePosition;

    public ulong LerpPosition;
    public ulong LastPosition;
}