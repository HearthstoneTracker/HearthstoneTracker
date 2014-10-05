// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultBalloonTip.xaml.cs" company="">
//   
// </copyright>
// <summary>
//   Interaction logic for DefaultBalloonTip.xaml
// </summary>
// --------------------------------------------------------------------------------------------------------------------

using System.Windows.Controls;
using System.Windows.Input;

namespace HearthCap.Shell.TrayIcon
{
    using Hardcodet.Wpf.TaskbarNotification;

    /// <summary>
    /// Interaction logic for DefaultBalloonTip.xaml
    /// </summary>
    public partial class DefaultBalloonTip
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DefaultBalloonTip"/> class.
        /// </summary>
        public DefaultBalloonTip()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// The layout root_ mouse down.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void LayoutRoot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
