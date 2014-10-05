// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CrashManager.cs" company="">
//   
// </copyright>
// <summary>
//   The crash manager.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.StartUp
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading.Tasks;
    using System.Windows.Threading;

    using HearthCap.Logging;

    using Microsoft.WindowsAPICodePack.ApplicationServices;

    using NLog;

    using Application = System.Windows.Application;
    using Tracker = HearthCap.Features.Analytics.Tracker;

    /// <summary>
    /// The crash manager.
    /// </summary>
    [Export(typeof(CrashManager))]
    public class CrashManager
    {
        /// <summary>
        /// The app log manager.
        /// </summary>
        private readonly IAppLogManager appLogManager;

        /// <summary>
        /// The log.
        /// </summary>
        private readonly static Logger Log = LogManager.GetCurrentClassLogger();

        /// <summary>
        /// Initializes a new instance of the <see cref="CrashManager"/> class.
        /// </summary>
        /// <param name="appLogManager">
        /// The app log manager.
        /// </param>
        [ImportingConstructor]
        public CrashManager(IAppLogManager appLogManager)
        {
            this.appLogManager = appLogManager;
        }

        /// <summary>
        /// The wire up.
        /// </summary>
        public void WireUp()
        {
            TaskScheduler.UnobservedTaskException += this.TaskSchedulerOnUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += this.CurrentDomainOnUnhandledException;
            Application.Current.DispatcherUnhandledException += this.ApplicationOnDispatcherUnhandledException;

            ApplicationRestartRecoveryManager.RegisterForApplicationRestart(new RestartSettings("-died", RestartRestrictions.None));
        }

        /// <summary>
        /// The task scheduler on unobserved task exception.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            this.HandleException(e.Exception);
        }

        /// <summary>
        /// The application on dispatcher unhandled exception.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void ApplicationOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            this.HandleException(e.Exception);
        }

        /// <summary>
        /// The current domain on unhandled exception.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            this.HandleException((Exception)e.ExceptionObject);
        }

        /// <summary>
        /// The handle exception.
        /// </summary>
        /// <param name="exception">
        /// The exception.
        /// </param>
        public void HandleException(Exception exception)
        {
            Log.Fatal(exception);

            Tracker.TrackEventAsync(Tracker.ErrorsCategory, "Fatal", exception.ToString(), 1);
            this.appLogManager.Flush();

            // var result = MessageBox.Show("An unhandled error occured. Please report this error.\nRestarting is recommended. Restart now?", "Unhandled error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            // if (result == DialogResult.Yes)
            // {
            // }
            // Environment.Exit(-1);
        }
    }
}