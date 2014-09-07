namespace HearthCap.Core.GameCapture.Logging
{
    using System;

    [Flags]
    public enum LogLevel
    {
        None = 0,
        Error = 1,
        Warn = 2,
        Info = 4,
        Diag = 8,
        Debug = 16,
        All = Error | Warn | Info | Diag | Debug,
    }
}