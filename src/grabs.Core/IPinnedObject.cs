using System;

namespace grabs.Core;

public interface IPinnedObject : IDisposable
{
    public nint Handle { get; }
}