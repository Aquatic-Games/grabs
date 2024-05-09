using System.Diagnostics;

namespace grabs.Core;

public static class GrabsLog
{
    public static event OnLogMessage LogMessage = delegate { };
    
    public static void Log(LogType type, string message)
    {
        LogMessage.Invoke(type, message);
    }
    
    public enum LogType
    {
        Verbose,
        Debug,
        Info,
        Warning,
        Error,
        Critical
    }
    
    public delegate void OnLogMessage(LogType type, string message);
}