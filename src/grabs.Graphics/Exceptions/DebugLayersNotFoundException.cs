namespace grabs.Graphics.Exceptions;

/// <summary>
/// An exception that is thrown when debugging is enabled, but the debugging tools for the backend are not installed.
/// </summary>
/// <param name="message">A message to show.</param>
public class DebugLayersNotFoundException(string message) : Exception(message);