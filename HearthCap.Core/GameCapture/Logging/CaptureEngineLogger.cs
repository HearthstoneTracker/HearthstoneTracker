// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureEngineLogger.cs" company="">
//   
// </copyright>
// <summary>
//   CaptureEngineLogger.cs
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.Logging
{
    // public class CaptureEngineLogger : ICaptureEngineLogger
    // {
    // private readonly Type type;

    // private readonly Logger NLog;

    // public CaptureEngineLogger(Type type)
    // {
    // this.type = type;
    // this.NLog = LogManager.GetLogger(type.Name);
    // }

    // public void Error(Exception exception)
    // {
    // this.Log(exception.Message, LogLevel.Error, exception);
    // NLog.Error(exception);
    // }

    // public void Error(string format, params object[] args)
    // {
    // this.Log(format, LogLevel.Error, null, args);
    // NLog.Error(String.Format(format, args));
    // }

    // public void Warn(string format, params object[] args)
    // {
    // this.Log(format, LogLevel.Warn, null, args);
    // NLog.Warn(String.Format(format, args));
    // }

    // public void Info(string format, params object[] args)
    // {
    // this.Log(format, LogLevel.Info, null, args);
    // NLog.Info(String.Format(format, args));
    // }

    // public void Diag(string format, params object[] args)
    // {
    // this.Log(format, LogLevel.Diag, null, args);
    // NLog.Trace(String.Format(format, args));
    // }

    // public void Diag(string message, object data)
    // {
    // #if DEBUG
    // this.Log(message, LogLevel.Diag, null, null);
    // NLog.Trace(message);
    // #endif
    // }

    // public void Debug(string format, params object[] args)
    // {
    // this.Log(format, LogLevel.Debug, null, args);
    // NLog.Debug(String.Format(format, args));
    // }

    // public static Action<string, LogLevel, object> LogAction = NullLog;

    // private static void NullLog(string message, LogLevel logLevel, object data)
    // {
    // }

    // protected void Log(string format, LogLevel level, object data, params object[] args)
    // {
    // LogAction(String.Format(format, args), level, data);
    // }

    // public static void Hook(Action<string, LogLevel, object> logAction)
    // {
    // var old = LogAction;
    // LogAction = (s, level, arg3) =>
    // {
    // old(s, level, arg3);
    // logAction(s, level, arg3);
    // };
    // }

    // public static Func<Type, ICaptureEngineLogger> GetLogger = (t) => new CaptureEngineLogger(t);
    // }
}