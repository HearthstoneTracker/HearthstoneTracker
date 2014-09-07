namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Core.GameCapture.Logging;
    using HearthCap.Data;
    using HearthCap.Shell.Flyouts;

    using MahApps.Metro.Controls;

    using NLog.Config;
    using NLog.Targets.Wrappers;

    using LogLevel = HearthCap.Core.GameCapture.Logging.LogLevel;

    [Export(typeof(IFlyout))]
    public class LogFlyoutViewModel : FlyoutViewModel
    {
        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly ICaptureEngine captureEngine;

        private BindableCollection<LogMessageModel> logMessages = new BindableCollection<LogMessageModel>();

        private bool loggingEnabled;

        private bool debugEnabled;

        private bool diagEnabled;

        private bool warnEnabled;

        private bool errorEnabled;

        private bool infoEnabled;

        private bool firstTime = true;

        private CaptureTarget captureTarget;

        private NLog.LogLevel currentLogLevel;

        private LoggingRule loggingRule;

        [ImportingConstructor]
        public LogFlyoutViewModel(
            Func<HearthStatsDbContext> dbContext,
            ICaptureEngine captureEngine)
        {
            this.dbContext = dbContext;
            this.captureEngine = captureEngine;
            this.Name = "log";
            this.Header = "Log";
            SetPosition(Position.Right);
            // CaptureEngineLogger.Hook(LogAction);
            this.warnEnabled = true;
            this.errorEnabled = true;
            this.infoEnabled = true;
            ConfigureCaptureTarget();
            RefreshLogSettings();
        }

        private void ConfigureCaptureTarget()
        {
            this.captureTarget = new CaptureTarget()
                             {
                                 Layout = "${date}|${level:uppercase=true}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}"
                             };
            captureTarget.LogReceived += target_LogReceived;
            var asyncWrapper = new AsyncTargetWrapper { Name = "CaptureTargetWrapper", WrappedTarget = captureTarget };

            NLog.LogManager.Configuration.AddTarget(asyncWrapper.Name, asyncWrapper);
            currentLogLevel = NLog.LogLevel.Info;
            loggingRule = new LoggingRule("*", currentLogLevel, asyncWrapper);
            NLog.LogManager.Configuration.LoggingRules.Insert(0, loggingRule);
            NLog.LogManager.ReconfigExistingLoggers();
            this.PropertyChanged += OnPropertyChanged;
#if DEBUG
            this.DebugEnabled = true;
#endif
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ErrorEnabled":
                case "WarnEnabled":
                case "InfoEnabled":
                case "DebugEnabled":
                case "DiagEnabled":
                    if (ErrorEnabled)
                    {
                        loggingRule.EnableLoggingForLevel(NLog.LogLevel.Error);
                        loggingRule.EnableLoggingForLevel(NLog.LogLevel.Fatal);
                    }
                    else
                    {
                        loggingRule.DisableLoggingForLevel(NLog.LogLevel.Error);
                        loggingRule.DisableLoggingForLevel(NLog.LogLevel.Fatal);
                    }
                    if (WarnEnabled)
                    {
                        loggingRule.EnableLoggingForLevel(NLog.LogLevel.Error);
                    }
                    else
                    {
                        loggingRule.DisableLoggingForLevel(NLog.LogLevel.Error);
                    }
                    if (InfoEnabled)
                    {
                        loggingRule.EnableLoggingForLevel(NLog.LogLevel.Info);
                    }
                    else
                    {
                        loggingRule.DisableLoggingForLevel(NLog.LogLevel.Info);
                    }
                    if (DebugEnabled)
                    {
                        loggingRule.EnableLoggingForLevel(NLog.LogLevel.Debug);
                    }
                    else
                    {
                        loggingRule.DisableLoggingForLevel(NLog.LogLevel.Debug);
                    }
                    if (DiagEnabled)
                    {
                        loggingRule.EnableLoggingForLevel(NLog.LogLevel.Trace);
                    }
                    else
                    {
                        loggingRule.DisableLoggingForLevel(NLog.LogLevel.Trace);
                    }
                    NLog.LogManager.ReconfigExistingLoggers();
                    break;                    
            }
        }

        void target_LogReceived(object sender, LogReceivedEventArgs e)
        {
            //if (e.Message.Level == NLog.LogLevel.Fatal && !ErrorEnabled) return;
            //if (e.Message.Level == NLog.LogLevel.Error && !ErrorEnabled) return;
            //if (e.Message.Level == NLog.LogLevel.Warn && !WarnEnabled) return;
            //if (e.Message.Level == NLog.LogLevel.Info && !InfoEnabled) return;
            //if (e.Message.Level == NLog.LogLevel.Debug && !DebugEnabled) return;
            //if (e.Message.Level == NLog.LogLevel.Trace && !DiagEnabled) return;

            if (this.logMessages.Count > 1000)
            {
                this.logMessages.RemoveAt(this.logMessages.Count - 1);
            }
            this.logMessages.Insert(0, e.Message);
        }

        private void LogAction(string s, LogLevel logLevel, object arg3)
        {
            if (!this.LogLevel.HasFlag(logLevel))
            {
                return;
            }

            if (this.logMessages.Count > 5000)
            {
                this.logMessages.RemoveAt(this.logMessages.Count - 1);
            }

            // this.logMessages.Insert(0, new LogMessageModel(s, logLevel, DateTime.Now));
        }

        public bool DebugEnabled
        {
            get
            {
                return this.debugEnabled;
            }
            set
            {
                if (value.Equals(this.debugEnabled))
                {
                    return;
                }
                this.debugEnabled = value;
                RefreshLogSettings();
                this.NotifyOfPropertyChange(() => this.DebugEnabled);
            }
        }

        public bool DiagEnabled
        {
            get
            {
                return this.diagEnabled;
            }
            set
            {
                if (value.Equals(this.diagEnabled))
                {
                    return;
                }
                this.diagEnabled = value;
                RefreshLogSettings();
                this.NotifyOfPropertyChange(() => this.DiagEnabled);
            }
        }

        public bool WarnEnabled
        {
            get
            {
                return this.warnEnabled;
            }
            set
            {
                if (value.Equals(this.warnEnabled))
                {
                    return;
                }
                this.warnEnabled = value;
                RefreshLogSettings();
                this.NotifyOfPropertyChange(() => this.WarnEnabled);
            }
        }

        public bool ErrorEnabled
        {
            get
            {
                return this.errorEnabled;
            }
            set
            {
                if (value.Equals(this.errorEnabled))
                {
                    return;
                }
                this.errorEnabled = value;
                RefreshLogSettings();
                this.NotifyOfPropertyChange(() => this.ErrorEnabled);
            }
        }

        public bool InfoEnabled
        {
            get
            {
                return this.infoEnabled;
            }
            set
            {
                if (value.Equals(this.infoEnabled))
                {
                    return;
                }
                this.infoEnabled = value;
                RefreshLogSettings();
                this.NotifyOfPropertyChange(() => this.InfoEnabled);
            }
        }

        public bool DiagVisible
        {
            get
            {
#if DEBUG
                return true;
#endif
                return false;
            }
        }

        private void RefreshLogSettings()
        {
            // TODO: enable/disable this in NLog so it also gets written to the log files.
        }

        public LogLevel LogLevel { get; set; }

        public BindableCollection<LogMessageModel> LogMessages
        {
            get
            {
                return this.logMessages;
            }
        }

        /// <summary>
        /// Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name="view"/>
        protected override void OnViewReady(object view)
        {
            if (!firstTime) return;
            firstTime = false;

            //    Dispatcher.CurrentDispatcher.Invoke(() =>
            //    {
            //        var target = new WpfRichTextBoxTarget
            //        {
            //            Name = "RichText",
            //            Layout =
            //                "[${longdate:useUTC=false}] :: [${level:uppercase=true}] :: ${logger}:${callsite} :: ${message} ${exception:innerFormat=tostring:maxInnerExceptionLevel=10:separator=,:format=tostring}",
            //            ControlName = "RichLog",
            //            FormName = GetType().Name,
            //            AutoScroll = true,
            //            MaxLines = 100000,
            //            UseDefaultRowColoringRules = true,
            //        };
            //        var asyncWrapper = new AsyncTargetWrapper { Name = "RichTextAsync", WrappedTarget = target };

            //        NLog.LogManager.Configuration.AddTarget(asyncWrapper.Name, asyncWrapper);
            //        NLog.LogManager.Configuration.LoggingRules.Insert(0, new LoggingRule("*", NLog.LogLevel.Trace, asyncWrapper));
            //        NLog.LogManager.ReconfigExistingLoggers();
            //    });
        }
    }
}