using System;
using System.ComponentModel.Composition;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Threading;
using HearthCap.Features.Analytics;
using HearthCap.Logging;
using Microsoft.WindowsAPICodePack.ApplicationServices;
using NLog;

namespace HearthCap.StartUp
{
    [Export(typeof(CrashManager))]
    public class CrashManager
    {
        private readonly IAppLogManager appLogManager;

        private static readonly NLog.Logger Log = LogManager.GetCurrentClassLogger();

        [ImportingConstructor]
        public CrashManager(IAppLogManager appLogManager)
        {
            this.appLogManager = appLogManager;
        }

        public void WireUp()
        {
            TaskScheduler.UnobservedTaskException += TaskSchedulerOnUnobservedTaskException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
            Application.Current.DispatcherUnhandledException += ApplicationOnDispatcherUnhandledException;

            ApplicationRestartRecoveryManager.RegisterForApplicationRestart(new RestartSettings("-died", RestartRestrictions.None));
        }

        private void TaskSchedulerOnUnobservedTaskException(object sender, UnobservedTaskExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private void ApplicationOnDispatcherUnhandledException(object sender, DispatcherUnhandledExceptionEventArgs e)
        {
            HandleException(e.Exception);
        }

        private void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            HandleException((Exception)e.ExceptionObject);
        }

        public void HandleException(Exception exception)
        {
            try
            {
                Log.Fatal(exception);
                Tracker.TrackEventAsync(Tracker.ErrorsCategory, "Fatal", exception.ToString(), 1);
                appLogManager.Flush();
            }
            catch (Exception)
            {
                // TODO: check, swallow any exceptions because of final actions after fatal error
            }

            //var result = MessageBox.Show("An unhandled error occured. Please report this error.\nRestarting is recommended. Restart now?", "Unhandled error", MessageBoxButtons.YesNo, MessageBoxIcon.Error);
            //if (result == DialogResult.Yes)
            //{
            //}
            // Environment.Exit(-1);
        }
    }
}
