// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogFlyoutViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The log flyout view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics.LogFlyout
{
    using System;
    using System.ComponentModel;
    using System.ComponentModel.Composition;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Data;
    using HearthCap.Shell.Flyouts;

    using MahApps.Metro.Controls;

    using NLog.Config;
    using NLog.Targets.Wrappers;

    using LogLevel = HearthCap.Core.GameCapture.Logging.LogLevel;
    using LogManager = NLog.LogManager;

    /// <summary>
    /// The log flyout view model.
    /// </summary>
    [Export(typeof(IFlyout))]
    public class LogFlyoutViewModel : FlyoutViewModel
    {
        /// <summary>
        /// The db context.
        /// </summary>
        private readonly Func<HearthStatsDbContext> dbContext;

        /// <summary>
        /// The capture engine.
        /// </summary>
        private readonly ICaptureEngine captureEngine;

        /// <summary>
        /// The log messages.
        /// </summary>
        private BindableCollection<LogMessageModel> logMessages = new BindableCollection<LogMessageModel>();

        /// <summary>
        /// The logging enabled.
        /// </summary>
        private bool loggingEnabled;

        /// <summary>
        /// The debug enabled.
        /// </summary>
        private bool debugEnabled;

        /// <summary>
        /// The diag enabled.
        /// </summary>
        private bool diagEnabled;

        /// <summary>
        /// The warn enabled.
        /// </summary>
        private bool warnEnabled;

        /// <summary>
        /// The error enabled.
        /// </summary>
        private bool errorEnabled;

        /// <summary>
        /// The info enabled.
        /// </summary>
        private bool infoEnabled;

        /// <summary>
        /// The first time.
        /// </summary>
        private bool firstTime = true;

        /// <summary>
        /// The capture target.
        /// </summary>
        private CaptureTarget captureTarget;

        /// <summary>
        /// The current log level.
        /// </summary>
        private NLog.LogLevel currentLogLevel;

        /// <summary>
        /// The logging rule.
        /// </summary>
        private LoggingRule loggingRule;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogFlyoutViewModel"/> class.
        /// </summary>
        /// <param name="dbContext">
        /// The db context.
        /// </param>
        /// <param name="captureEngine">
        /// The capture engine.
        /// </param>
        [ImportingConstructor]
        public LogFlyoutViewModel(
            Func<HearthStatsDbContext> dbContext, 
            ICaptureEngine captureEngine)
        {
            this.dbContext = dbContext;
            this.captureEngine = captureEngine;
            this.Name = "log";
            this.Header = "Log";
            this.SetPosition(Position.Right);

            // CaptureEngineLogger.Hook(LogAction);
            this.warnEnabled = true;
            this.errorEnabled = true;
            this.infoEnabled = true;
            this.ConfigureCaptureTarget();
            this.RefreshLogSettings();
        }

        /// <summary>
        /// The configure capture target.
        /// </summary>
        private void ConfigureCaptureTarget()
        {
            this.captureTarget = new CaptureTarget {
                                 Layout = "${date}|${level:uppercase=true}|${logger}|${message}${onexception:inner=${newline}${exception:format=tostring}}"
                             };
            this.captureTarget.LogReceived += this.target_LogReceived;
            var asyncWrapper = new AsyncTargetWrapper { Name = "CaptureTargetWrapper", WrappedTarget = this.captureTarget };

            LogManager.Configuration.AddTarget(asyncWrapper.Name, asyncWrapper);
            this.currentLogLevel = NLog.LogLevel.Info;
            this.loggingRule = new LoggingRule("*", this.currentLogLevel, asyncWrapper);
            LogManager.Configuration.LoggingRules.Insert(0, this.loggingRule);
            LogManager.ReconfigExistingLoggers();
            this.PropertyChanged += this.OnPropertyChanged;
#if DEBUG
            this.DebugEnabled = true;
#endif
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "ErrorEnabled":
                case "WarnEnabled":
                case "InfoEnabled":
                case "DebugEnabled":
                case "DiagEnabled":
                    if (this.ErrorEnabled)
                    {
                        this.loggingRule.EnableLoggingForLevel(NLog.LogLevel.Error);
                        this.loggingRule.EnableLoggingForLevel(NLog.LogLevel.Fatal);
                    }
                    else
                    {
                        this.loggingRule.DisableLoggingForLevel(NLog.LogLevel.Error);
                        this.loggingRule.DisableLoggingForLevel(NLog.LogLevel.Fatal);
                    }

                    if (this.WarnEnabled)
                    {
                        this.loggingRule.EnableLoggingForLevel(NLog.LogLevel.Error);
                    }
                    else
                    {
                        this.loggingRule.DisableLoggingForLevel(NLog.LogLevel.Error);
                    }

                    if (this.InfoEnabled)
                    {
                        this.loggingRule.EnableLoggingForLevel(NLog.LogLevel.Info);
                    }
                    else
                    {
                        this.loggingRule.DisableLoggingForLevel(NLog.LogLevel.Info);
                    }

                    if (this.DebugEnabled)
                    {
                        this.loggingRule.EnableLoggingForLevel(NLog.LogLevel.Debug);
                    }
                    else
                    {
                        this.loggingRule.DisableLoggingForLevel(NLog.LogLevel.Debug);
                    }

                    if (this.DiagEnabled)
                    {
                        this.loggingRule.EnableLoggingForLevel(NLog.LogLevel.Trace);
                    }
                    else
                    {
                        this.loggingRule.DisableLoggingForLevel(NLog.LogLevel.Trace);
                    }

                    LogManager.ReconfigExistingLoggers();
                    break;                    
            }
        }

        /// <summary>
        /// The target_ log received.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        void target_LogReceived(object sender, LogReceivedEventArgs e)
        {
            // if (e.Message.Level == NLog.LogLevel.Fatal && !ErrorEnabled) return;
            // if (e.Message.Level == NLog.LogLevel.Error && !ErrorEnabled) return;
            // if (e.Message.Level == NLog.LogLevel.Warn && !WarnEnabled) return;
            // if (e.Message.Level == NLog.LogLevel.Info && !InfoEnabled) return;
            // if (e.Message.Level == NLog.LogLevel.Debug && !DebugEnabled) return;
            // if (e.Message.Level == NLog.LogLevel.Trace && !DiagEnabled) return;
            if (this.logMessages.Count > 1000)
            {
                this.logMessages.RemoveAt(this.logMessages.Count - 1);
            }

            this.logMessages.Insert(0, e.Message);
        }

        /// <summary>
        /// The log action.
        /// </summary>
        /// <param name="s">
        /// The s.
        /// </param>
        /// <param name="logLevel">
        /// The log level.
        /// </param>
        /// <param name="arg3">
        /// The arg 3.
        /// </param>
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

        /// <summary>
        /// Gets or sets a value indicating whether debug enabled.
        /// </summary>
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
                this.RefreshLogSettings();
                this.NotifyOfPropertyChange(() => this.DebugEnabled);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether diag enabled.
        /// </summary>
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
                this.RefreshLogSettings();
                this.NotifyOfPropertyChange(() => this.DiagEnabled);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether warn enabled.
        /// </summary>
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
                this.RefreshLogSettings();
                this.NotifyOfPropertyChange(() => this.WarnEnabled);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether error enabled.
        /// </summary>
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
                this.RefreshLogSettings();
                this.NotifyOfPropertyChange(() => this.ErrorEnabled);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether info enabled.
        /// </summary>
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
                this.RefreshLogSettings();
                this.NotifyOfPropertyChange(() => this.InfoEnabled);
            }
        }

        /// <summary>
        /// Gets a value indicating whether diag visible.
        /// </summary>
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

        /// <summary>
        /// The refresh log settings.
        /// </summary>
        private void RefreshLogSettings()
        {
            // TODO: enable/disable this in NLog so it also gets written to the log files.
        }

        /// <summary>
        /// Gets or sets the log level.
        /// </summary>
        public LogLevel LogLevel { get; set; }

        /// <summary>
        /// Gets the log messages.
        /// </summary>
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
        /// <param name="view">
        /// </param>
        protected override void OnViewReady(object view)
        {
            if (!this.firstTime) return;
            this.firstTime = false;

            // Dispatcher.CurrentDispatcher.Invoke(() =>
            // {
            // var target = new WpfRichTextBoxTarget
            // {
            // Name = "RichText",
            // Layout =
            // "[${longdate:useUTC=false}] :: [${level:uppercase=true}] :: ${logger}:${callsite} :: ${message} ${exception:innerFormat=tostring:maxInnerExceptionLevel=10:separator=,:format=tostring}",
            // ControlName = "RichLog",
            // FormName = GetType().Name,
            // AutoScroll = true,
            // MaxLines = 100000,
            // UseDefaultRowColoringRules = true,
            // };
            // var asyncWrapper = new AsyncTargetWrapper { Name = "RichTextAsync", WrappedTarget = target };

            // NLog.LogManager.Configuration.AddTarget(asyncWrapper.Name, asyncWrapper);
            // NLog.LogManager.Configuration.LoggingRules.Insert(0, new LoggingRule("*", NLog.LogLevel.Trace, asyncWrapper));
            // NLog.LogManager.ReconfigExistingLoggers();
            // });
        }
    }
}