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
    using HearthCap.StartUp;

    [Export(typeof(IStartupTask))]
    public class Startup : IStartupTask,
        IHandle<ShellReady>
    {
        private readonly IEventAggregator events;

        private readonly SettingsManager settingsManager;

        private readonly ICaptureEngine captureEngine;

        private readonly ILogCaptureEngine logCaptureEngine;

        private CrashManager crashManager;

        [ImportingConstructor]
        public Startup(
            IEventAggregator events,
            SettingsManager settingsManager,
            ICaptureEngine captureEngine,
            ILogCaptureEngine logCaptureEngine,
            CrashManager crashManager)
        {
            this.events = events;
            this.settingsManager = settingsManager;
            this.captureEngine = captureEngine;
            this.logCaptureEngine = logCaptureEngine;
            this.crashManager = crashManager;
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
            using (var reg = new EngineRegistrySettings())
            {
                captureEngine.CaptureMethod = reg.CaptureMethod;
                captureEngine.Speed = (int)reg.Speed;
            }

            this.captureEngine.StartAsync();
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