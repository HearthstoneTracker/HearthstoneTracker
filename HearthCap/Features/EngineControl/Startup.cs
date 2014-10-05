// --------------------------------------------------------------------------------------------------------------------
// <copyright file="Startup.cs" company="">
//   
// </copyright>
// <summary>
//   The startup.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.EngineControl
{
    using System;
    using System.ComponentModel.Composition;
    using System.Windows;

    using Caliburn.Micro;

    using HearthCap.Core.GameCapture;
    using HearthCap.Shell.Events;
    using HearthCap.Shell.Settings;
    using HearthCap.StartUp;

    /// <summary>
    /// The startup.
    /// </summary>
    [Export(typeof(IStartupTask))]
    public class Startup : IStartupTask, 
        IHandle<ShellReady>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private readonly IEventAggregator events;

        /// <summary>
        /// The settings manager.
        /// </summary>
        private readonly SettingsManager settingsManager;

        /// <summary>
        /// The capture engine.
        /// </summary>
        private readonly ICaptureEngine captureEngine;

        /// <summary>
        /// The log capture engine.
        /// </summary>
        private readonly ILogCaptureEngine logCaptureEngine;

        /// <summary>
        /// The crash manager.
        /// </summary>
        private CrashManager crashManager;

        /// <summary>
        /// Initializes a new instance of the <see cref="Startup"/> class.
        /// </summary>
        /// <param name="events">
        /// The events.
        /// </param>
        /// <param name="settingsManager">
        /// The settings manager.
        /// </param>
        /// <param name="captureEngine">
        /// The capture engine.
        /// </param>
        /// <param name="logCaptureEngine">
        /// The log capture engine.
        /// </param>
        /// <param name="crashManager">
        /// The crash manager.
        /// </param>
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
            Application.Current.Exit += this.CurrentOnExit;
        }

        /// <summary>
        /// The on unhandled exception.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {

            this.crashManager.HandleException((Exception)e.ExceptionObject);
        }

        /// <summary>
        /// The run.
        /// </summary>
        public void Run()
        {
            this.events.Subscribe(this);
        }

        /// <summary>
        /// Handles the message.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        public void Handle(ShellReady message)
        {
            using (var reg = new EngineRegistrySettings())
            {
                this.captureEngine.CaptureMethod = reg.CaptureMethod;
                this.captureEngine.Speed = reg.Speed;
            }

            this.captureEngine.StartAsync();
        }

        /// <summary>
        /// The current on exit.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="exitEventArgs">
        /// The exit event args.
        /// </param>
        private void CurrentOnExit(object sender, ExitEventArgs exitEventArgs)
        {
            if (this.captureEngine.IsRunning)
            {
                this.captureEngine.Stop();
            }
        }
    }
}