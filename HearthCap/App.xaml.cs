using System;
using System.Linq;
using System.Threading;
using System.Windows;
using HearthCap.UI.Controls;

namespace HearthCap
{
    /// <summary>
    ///     Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private static Mutex mutex;

        public App()
        {
            var args = Environment.GetCommandLineArgs();

            if (!args.Any(x => x.Contains("-restarting")))
            {
                bool createdNew;
                mutex = new Mutex(true, @"HearthCap", out createdNew);
                if (!createdNew)
                {
                    mutex = null;
                    var result = MessageBox.Show("Already running, exiting now.", "Already running", MessageBoxButton.OK, MessageBoxImage.Information);
                    if (result == MessageBoxResult.OK)
                    {
                        Current.Shutdown();
                        return;
                    }
                }
            }

            var isLogon = args.Any(x => x.Contains("-logon"));
            if (!isLogon)
            {
                var splash = new DelayedSplashScreen("Resources/logo1.png");
                splash.Show(TimeSpan.FromMilliseconds(500));
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
        }

        /// <summary>
        ///     Raises the <see cref="E:System.Windows.Application.Exit" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.Windows.ExitEventArgs" /> that contains the event data.</param>
        protected override void OnExit(ExitEventArgs e)
        {
            if (mutex != null)
            {
                mutex.ReleaseMutex();
                mutex.Dispose();
            }
            base.OnExit(e);
        }
    }
}
