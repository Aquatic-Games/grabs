namespace grabs.Graphics.Exceptions;

public class NoBackendsException()
    : Exception("No backends have been registered. At least 1 backend must be registered by calling Instance.RegisterBackend()");