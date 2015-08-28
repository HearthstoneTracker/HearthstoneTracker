namespace HearthCap.Updater
{
    using System;
    using System.IO;
    using System.Windows.Forms;

    using NLog;
    using NLog.Config;
    using NLog.Targets;

    class Program
    {
        static void Main(string[] args)
        {
            if (args.Length <= 0)
                return;

            var config = new LoggingConfiguration();
            var logfile = new FileTarget();
            var logdir = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "HearthstoneTracker");
            logdir = Path.Combine(logdir, "logs");
            var logfilename = Path.Combine(logdir, "updater.${date:format=yyyy-MM-dd}.txt");
            logfile.FileName = logfilename;
            logfile.CreateDirs = true;
            logfile.MaxArchiveFiles = 7;
            logfile.ArchiveEvery = FileArchivePeriod.Day;
            logfile.ConcurrentWrites = true;
            logfile.Layout =
                "${longdate}|${level:uppercase=true}|thread:${threadid}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}";

            config.AddTarget("logfile", logfile);
            config.LoggingRules.Add(new LoggingRule("*", LogLevel.Debug, logfile));
            LogManager.Configuration = config;
            
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new UpdateProgress(args));             
        }
    }
}
