using System.Runtime.CompilerServices;
// ReSharper disable ExplicitCallerInfoArgument

namespace grabs.Core;

public static class GrabsLog
{
    public static event OnLogMessage LogMessage = delegate { };

    public static void Log(Severity severity, string message, [CallerLineNumber] int line = 0,
        [CallerFilePath] string file = "")
    {
        LogMessage(severity, message, line, file);
    }

    public static void Log(string message, [CallerLineNumber] int line = 0, [CallerFilePath] string file = "")
        => Log(Severity.Verbose, message, line, file);

    public enum Severity
    {
        Verbose,
        Debug,
        Info,
        Warning,
        Error
    }

    public delegate void OnLogMessage(Severity severity, string message, int line, string file);
}