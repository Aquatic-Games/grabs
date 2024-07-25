using System.Diagnostics;

namespace grabs.Core;

public static class GrabsLog
{
    public static event OnLogMessage LogMessage = delegate { };
    
    public static void Log(Severity severity, string message)
    {
        LogMessage.Invoke(severity, message);
    }
    
    public enum Severity
    {
        Verbose,
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }
    
    public delegate void OnLogMessage(Severity type, string message);
}