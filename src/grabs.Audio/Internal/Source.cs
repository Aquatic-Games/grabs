using System.Collections.Generic;

namespace grabs.Audio.Internal;

internal struct Source
{
    public Queue<ulong> QueuedBuffers;

    public bool Playing;

    public ulong Position;
    public double FinePosition;

    public Source()
    {
        QueuedBuffers = new Queue<ulong>();
        Playing = false;
    }
}