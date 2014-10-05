// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DelayedSplashScreen.cs" company="">
//   
// </copyright>
// <summary>
//   The delayed splash screen.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.UI.Controls
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    /// <summary>
    /// The delayed splash screen.
    /// </summary>
    public class DelayedSplashScreen : SplashScreen
    {
        /// <summary>
        /// The dt.
        /// </summary>
        private DispatcherTimer dt;

        /// <summary>
        /// Initializes a new instance of the <see cref="DelayedSplashScreen"/> class.
        /// </summary>
        /// <param name="resourceName">
        /// The resource name.
        /// </param>
        public DelayedSplashScreen(string resourceName)
            : base(resourceName)
        { }

        /// <summary>
        /// Shows the splash screen
        /// </summary>
        /// <param name="minDur">
        /// The minimum duration this splash should show for
        /// </param>
        /// <param name="topmost">
        /// True if the splash screen should appear 
        /// top most in windows. Recommended to be true, to ensure it does not appear 
        /// behind the loaded main window
        /// </param>
        public void Show(TimeSpan minDur, bool topmost = true)
        {
            if (this.dt == null)
            {
                // prevent calling twice
                this.Show(false, topmost);
                this.dt = new DispatcherTimer(minDur, DispatcherPriority.Loaded, 
                            this.CloseAfterDelay, Dispatcher.CurrentDispatcher);
            }
        }

        /// <summary>
        /// The close after delay.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void CloseAfterDelay(object sender, EventArgs e)
        {
            this.dt.Stop();
            this.Close(TimeSpan.FromMilliseconds(300));
        }
    }
}