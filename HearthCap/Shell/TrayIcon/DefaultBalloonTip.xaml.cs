using System.Windows.Controls;
using System.Windows.Input;
using Hardcodet.Wpf.TaskbarNotification;

namespace HearthCap.Shell.TrayIcon
{
    /// <summary>
    ///     Interaction logic for DefaultBalloonTip.xaml
    /// </summary>
    public partial class DefaultBalloonTip : UserControl
    {
        public DefaultBalloonTip()
        {
            InitializeComponent();
        }

        private void LayoutRoot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            var taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
