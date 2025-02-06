using System.Runtime.CompilerServices;
// ReSharper disable ExplicitCallerInfoArgument

namespace grabs.Core;

public static class GrabsLog
{
    public static event OnLogMessage LogMessage;

    static GrabsLog()
    {
        LogMessage = delegate { };
    }
    
    public static void Log(Severity severity, Source source, string message, [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        LogMessage(severity, source, message, file, line);
    }

    public static void Log(string message, [CallerFilePath] string file = "", [CallerLineNumber] int line = 0)
        => Log(Severity.Verbose, Source.General, message, file, line);

    public static void Log(Severity severity, string message, [CallerFilePath] string file = "",
        [CallerLineNumber] int line = 0)
    {
        Log(severity, Source.General, message, file, line);
    }
    
    public enum Severity
    {
        Verbose,
        Info,
        Warning,
        Error
    }

    public enum Source
    {
        General,
        Validation,
        Performance,
        Other
    }

    public delegate void OnLogMessage(Severity severity, Source source, string message, string file, int line);
}