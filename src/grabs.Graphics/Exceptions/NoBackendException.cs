namespace grabs.Graphics.Exceptions;

public class NoBackendException : Exception
{
    public NoBackendException() : base("No backend available!") { }
}