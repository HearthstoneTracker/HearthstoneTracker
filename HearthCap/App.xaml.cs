// --------------------------------------------------------------------------------------------------------------------
// <copyright file="App.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for App.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap
{
    using System;
    using System.Linq;
    using System.Threading;
    using System.Windows;

    using HearthCap.UI.Controls;

    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App
    {
        /// <summary>
        /// The mutex.
        /// </summary>
        private static Mutex mutex;

        /// <summary>
        /// Initializes a new instance of the <see cref="App"/> class.
        /// </summary>
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

            bool isLogon = args.Any(x => x.Contains("-logon"));
            if (!isLogon)
            {
                var splash = new DelayedSplashScreen("Resources/logo1.png");
                splash.Show(TimeSpan.FromMilliseconds(500));
            }
        }

        /// <summary>
        /// Raises the <see cref="E:System.Windows.Application.Exit"/> event.
        /// </summary>
        /// <param name="e">
        /// An <see cref="T:System.Windows.ExitEventArgs"/> that contains the event data.
        /// </param>
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
