using System;

namespace grabs.Graphics.GL43;

public class GL43Surface : Surface
{
    public readonly Action<int> PresentFunc;

    public GL43Surface(Action<int> presentFunc)
    {
        PresentFunc = presentFunc;
    }
    
    public override void Dispose() { }
}