using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using Caliburn.Micro;
using HearthCap.Core.GameCapture;
using HearthCap.Data;
using HearthCap.Shell.Flyouts;
using MahApps.Metro.Controls;
using NLog;
using NLog.Config;
using NLog.Targets.Wrappers;
using LogManager = NLog.LogManager;

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    [Export(typeof(IFlyout))]
    public class LogFlyoutViewModel : FlyoutViewModel
    {
        private readonly Func<HearthStatsDbContext> dbContext;

        private readonly ICaptureEngine captureEngine;

        private readonly BindableCollection<LogMessageModel> logMessages = new BindableCollection<LogMessageModel>();

        private bool debugEnabled;

        private bool diagEnabled;

        private bool warnEnabled;

        private bool errorEnabled;

        private bool infoEnabled;

        private bool firstTime = true;

        private CaptureTarget captureTarget;

        private LogLevel currentLogLevel;

        private LoggingRule loggingRule;

        [ImportingConstructor]
        public LogFlyoutViewModel(
            Func<HearthStatsDbContext> dbContext,
            ICaptureEngine captureEngine)
        {
            this.dbContext = dbContext;
            this.captureEngine = captureEngine;
            Name = "log";
            Header = "Log";
            SetPosition(Position.Right);
            // CaptureEngineLogger.Hook(LogAction);
            warnEnabled = true;
            errorEnabled = true;
            infoEnabled = true;
            ConfigureCaptureTarget();
            RefreshLogSettings();
        }

        private void ConfigureCaptureTarget()
        {
            captureTarget = new CaptureTarget
                {
                    Layout = "${date}|${level:uppercase=true}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}"
                };
            captureTarget.LogReceived += target_LogReceived;
            var asyncWrapper = new AsyncTargetWrapper { Name = "CaptureTargetWrapper", WrappedTarget = captureTarget };

            LogManager.Configuration.AddTarget(asyncWrapper.Name, asyncWrapper);
            currentLogLevel = NLog.LogLevel.Info;
            loggingRule = new LoggingRule("*", currentLogLevel, asyncWrapper);
            LogManager.Configuration.LoggingRules.Insert(0, loggingRule);
            LogManager.ReconfigExistingLoggers();
            PropertyChanged += OnPropertyChanged;
#if DEBUG
            DebugEnabled = true;
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
                    LogManager.ReconfigExistingLoggers();
                    break;
            }
        }

        private void target_LogReceived(object sender, LogReceivedEventArgs e)
        {
            //if (e.Message.Level == NLog.LogLevel.Fatal && !ErrorEnabled) return;
            //if (e.Message.Level == NLog.LogLevel.Error && !ErrorEnabled) return;
            //if (e.Message.Level == NLog.LogLevel.Warn && !WarnEnabled) return;
            //if (e.Message.Level == NLog.LogLevel.Info && !InfoEnabled) return;
            //if (e.Message.Level == NLog.LogLevel.Debug && !DebugEnabled) return;
            //if (e.Message.Level == NLog.LogLevel.Trace && !DiagEnabled) return;

            if (logMessages.Count > 1000)
            {
                logMessages.RemoveAt(logMessages.Count - 1);
            }
            logMessages.Insert(0, e.Message);
        }

        private void LogAction(string s, HearthCap.Core.GameCapture.Logging.LogLevel logLevel, object arg3)
        {
            if (!LogLevel.HasFlag(logLevel))
            {
                return;
            }

            if (logMessages.Count > 5000)
            {
                logMessages.RemoveAt(logMessages.Count - 1);
            }

            // this.logMessages.Insert(0, new LogMessageModel(s, logLevel, DateTime.Now));
        }

        public bool DebugEnabled
        {
            get { return debugEnabled; }
            set
            {
                if (value.Equals(debugEnabled))
                {
                    return;
                }
                debugEnabled = value;
                RefreshLogSettings();
                NotifyOfPropertyChange(() => DebugEnabled);
            }
        }

        public bool DiagEnabled
        {
            get { return diagEnabled; }
            set
            {
                if (value.Equals(diagEnabled))
                {
                    return;
                }
                diagEnabled = value;
                RefreshLogSettings();
                NotifyOfPropertyChange(() => DiagEnabled);
            }
        }

        public bool WarnEnabled
        {
            get { return warnEnabled; }
            set
            {
                if (value.Equals(warnEnabled))
                {
                    return;
                }
                warnEnabled = value;
                RefreshLogSettings();
                NotifyOfPropertyChange(() => WarnEnabled);
            }
        }

        public bool ErrorEnabled
        {
            get { return errorEnabled; }
            set
            {
                if (value.Equals(errorEnabled))
                {
                    return;
                }
                errorEnabled = value;
                RefreshLogSettings();
                NotifyOfPropertyChange(() => ErrorEnabled);
            }
        }

        public bool InfoEnabled
        {
            get { return infoEnabled; }
            set
            {
                if (value.Equals(infoEnabled))
                {
                    return;
                }
                infoEnabled = value;
                RefreshLogSettings();
                NotifyOfPropertyChange(() => InfoEnabled);
            }
        }

        public bool DiagVisible
        {
            get
            {
#pragma warning disable 162 // unreachable code detected
#if DEBUG
                return true;
#endif
                return false;
#pragma warning restore 162
            }
        }

        private void RefreshLogSettings()
        {
            // TODO: enable/disable this in NLog so it also gets written to the log files.
        }

        public HearthCap.Core.GameCapture.Logging.LogLevel LogLevel { get; set; }

        public BindableCollection<LogMessageModel> LogMessages
        {
            get { return logMessages; }
        }

        /// <summary>
        ///     Called the first time the page's LayoutUpdated event fires after it is navigated to.
        /// </summary>
        /// <param name="view" />
        protected override void OnViewReady(object view)
        {
            if (!firstTime)
            {
                return;
            }
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
