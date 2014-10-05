// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AppLogManager.cs" company="">
//   
// </copyright>
// <summary>
//   The app log manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Logging
{
    using System;
    using System.ComponentModel.Composition;
    using System.IO;

    using NLog.Config;
    using NLog.Targets;
    using NLog.Targets.Wrappers;

    using LogLevel = NLog.LogLevel;
    using LogManager = NLog.LogManager;

    /// <summary>
    /// The app log manager.
    /// </summary>
    [Export(typeof(IAppLogManager))]
    public class AppLogManager : IAppLogManager
    {
        /// <summary>
        /// The logfile.
        /// </summary>
        private FileTarget logfile;

        /// <summary>
        /// Initializes a new instance of the <see cref="AppLogManager"/> class.
        /// </summary>
        [ImportingConstructor]
        public AppLogManager()
        {
        }

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="logFilesDirectory">
        /// The log files directory.
        /// </param>
        public void Initialize(string logFilesDirectory)
        {
            var config = new LoggingConfiguration();

            this.logfile = new FileTarget();
            var logfilename = Path.Combine(logFilesDirectory, "${date:format=yyyy-MM-dd}.txt");
            this.logfile.FileName = logfilename;
            this.logfile.CreateDirs = true;
            this.logfile.MaxArchiveFiles = 7;
            this.logfile.ArchiveEvery = FileArchivePeriod.Day;
            this.logfile.ConcurrentWrites = true;
            this.logfile.Layout =
                "${longdate}|${level:uppercase=true}|thread:${threadid}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}";

            var asyncTarget = new AsyncTargetWrapper(this.logfile) { OverflowAction = AsyncTargetWrapperOverflowAction.Grow };
            config.AddTarget("logfile", asyncTarget);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, asyncTarget));

            // #if DEBUG
            // var tracelogfile = new FileTarget();
            // tracelogfile.FileName = Path.Combine(logFilesDirectory, "${date:format=yyyy-MM-dd}_Trace.txt");
            // tracelogfile.CreateDirs = true;
            // tracelogfile.MaxArchiveFiles = 7;
            // tracelogfile.ArchiveEvery = FileArchivePeriod.Day;
            // tracelogfile.ConcurrentWrites = true;
            // tracelogfile.Layout =
            // "${longdate}|${level:uppercase=true}|thread:${threadid}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}";

            // var asyncTarget2 = new AsyncTargetWrapper(tracelogfile)
            // {
            // OverflowAction = AsyncTargetWrapperOverflowAction.Grow
            // };
            // config.AddTarget("tracelogfile", asyncTarget2);
            // config.LoggingRules.Add(new LoggingRule("*", LogLevel.Trace, asyncTarget2));
            // #endif
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
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// The flush.
        /// </summary>
        public void Flush()
        {
            try
            {
                this.logfile.Flush(ex => { });
            }
            catch (Exception ex)
            {
            }
        }
    }
}