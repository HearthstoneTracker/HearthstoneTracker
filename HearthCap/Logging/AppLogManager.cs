namespace HearthCap.Logging
{
    using System;
    using System.ComponentModel.Composition;
    using System.IO;

    using NLog;
    using NLog.Config;
    using NLog.Targets;
    using NLog.Targets.Wrappers;

    using LogLevel = NLog.LogLevel;
    using LogManager = NLog.LogManager;

    [Export(typeof(IAppLogManager))]
    public sealed class AppLogManager : IAppLogManager
    {
        [ImportingConstructor]
        public AppLogManager()
        {
        }

        public void Initialize(string logFilesDirectory)
        {
            var config = new LoggingConfiguration();

            var logfile = new FileTarget();
            var logfilename = Path.Combine(logFilesDirectory, "${date:format=yyyy-MM-dd}.txt");
            logfile.FileName = logfilename;
            logfile.CreateDirs = true;
            logfile.MaxArchiveFiles = 7;
            logfile.ArchiveEvery = FileArchivePeriod.Day;
            logfile.ConcurrentWrites = true;
            logfile.Layout =
                "${longdate}|${level:uppercase=true}|thread:${threadid}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}";

            var asyncTarget = new AsyncTargetWrapper(logfile)
                                  {
                                      OverflowAction = AsyncTargetWrapperOverflowAction.Grow
                                  };
            config.AddTarget("logfile", asyncTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, asyncTarget));

//#if DEBUG
//            var tracelogfile = new FileTarget();
//            tracelogfile.FileName = Path.Combine(logFilesDirectory, "${date:format=yyyy-MM-dd}_Trace.txt");
//            tracelogfile.CreateDirs = true;
//            tracelogfile.MaxArchiveFiles = 7;
//            tracelogfile.ArchiveEvery = FileArchivePeriod.Day;
//            tracelogfile.ConcurrentWrites = true;
//            tracelogfile.Layout =
//                "${longdate}|${level:uppercase=true}|thread:${threadid}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}";

//            var asyncTarget2 = new AsyncTargetWrapper(tracelogfile)
//                                   {
//                                       OverflowAction = AsyncTargetWrapperOverflowAction.Grow
//                                   };
//            config.AddTarget("tracelogfile", asyncTarget2);
//            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, asyncTarget2));
//#endif

            LogManager.Configuration = config;
            // Caliburn.Micro.LogManager.GetLog = type => new NLogger(type);
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            try
            {
                LogManager.Shutdown();
            }
            catch (Exception)
            {
                // TODO: check, swallow exceptions during shutdown/dispose
            }
        }

        public void Flush()
        {
            LogManager.Flush();
        }
    }
}