using System.Collections.Generic;

namespace grabs.Audio.Internal;

internal struct Source
{
    public Queue<ulong> QueuedBuffers;

    public Source()
    {
        QueuedBuffers = new Queue<ulong>();
    }
}