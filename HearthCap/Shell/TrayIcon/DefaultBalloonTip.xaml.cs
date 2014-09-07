using System.Windows.Controls;
using System.Windows.Input;

namespace HearthCap.Shell.TrayIcon
{
    using Hardcodet.Wpf.TaskbarNotification;

    /// <summary>
    /// Interaction logic for DefaultBalloonTip.xaml
    /// </summary>
    public partial class DefaultBalloonTip : UserControl
    {
        public DefaultBalloonTip()
        {
            InitializeComponent();
        }

        private void LayoutRoot_MouseDown(object sender, MouseButtonEventArgs e)
        {
            TaskbarIcon taskbarIcon = TaskbarIcon.GetParentTaskbarIcon(this);
            taskbarIcon.CloseBalloon();
        }
    }
}
