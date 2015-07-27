namespace HearthCap.Features.EngineControl
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using System.Windows;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Shell.Events;
    using HearthCap.Shell.Settings;
    using HearthCap.Shell.UserPreferences;
    using HearthCap.StartUp;

    [Export(typeof(IStartupTask))]
    public class Startup : IStartupTask,
        IHandle<ShellReady>
    {
        private readonly IEventAggregator events;

        private readonly SettingsManager settingsManager;

        private readonly ICaptureEngine captureEngine;

        private readonly ILogCaptureEngine logCaptureEngine;

        private readonly UserPreferences _userPreferences;

        private CrashManager crashManager;

        [ImportingConstructor]
        public Startup(
            IEventAggregator events,
            SettingsManager settingsManager,
            ICaptureEngine captureEngine,
            ILogCaptureEngine logCaptureEngine,
            CrashManager crashManager,
            UserPreferences userPreferences)
        {
            this.events = events;
            this.settingsManager = settingsManager;
            this.captureEngine = captureEngine;
            this.logCaptureEngine = logCaptureEngine;
            this.crashManager = crashManager;
            this._userPreferences = userPreferences;
            captureEngine.UnhandledException += this.OnUnhandledException;
            Application.Current.Exit += CurrentOnExit;
        }

        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            crashManager.HandleException((Exception)e.ExceptionObject);
        }

        public void Run()
        {
            this.events.Subscribe(this);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">The message.</param>
        public void Handle(ShellReady message)
        {
            if (this._userPreferences.AutoAttachToHearthstone)
            {
                using (var reg = new EngineRegistrySettings())
                {
                    captureEngine.CaptureMethod = reg.CaptureMethod;
                    captureEngine.Speed = (int)reg.Speed;
                }

                this.captureEngine.StartAsync();
            }
        }

        private void CurrentOnExit(object sender, ExitEventArgs exitEventArgs)
        {
            if (captureEngine.IsRunning)
            {
                captureEngine.Stop();
            }
        }
    }
}