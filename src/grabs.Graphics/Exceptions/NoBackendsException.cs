namespace grabs.Graphics.Exceptions;

/// <summary>
/// An exception that is thrown when no backends have been registered.
/// </summary>
public class NoBackendsException()
    : Exception("No backends have been registered. At least 1 backend must be registered by calling Instance.RegisterBackend()");