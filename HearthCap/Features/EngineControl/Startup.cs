namespace HearthCap.Features.EngineControl
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Shell.Events;
    using HearthCap.StartUp;

    [Export(typeof(IStartupTask))]
    public class Startup : IStartupTask,
        IHandle<ShellReady>
    {
        private readonly IEventAggregator events;

        private readonly ICaptureEngine captureEngine;

        [ImportingConstructor]
        public Startup(
            IEventAggregator events,
            ICaptureEngine captureEngine,
            CrashManager crashManager)
        {
            this.events = events;
            this.captureEngine = captureEngine;
            captureEngine.UnhandledException += (s, e) => crashManager.HandleException(e.ExceptionObject as Exception);
            Application.Current.Exit += CurrentOnExit;
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
            bool autoStart;
            using (var reg = new EngineRegistrySettings())
            {
                captureEngine.CaptureMethod = reg.CaptureMethod;
                captureEngine.Speed = (int)reg.Speed;
                autoStart = reg.AutoStart;
            }

            if (autoStart)
            {
                captureEngine.StartAsync();
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