using System.Windows;

namespace HearthCap.Shell.Events
{
    public class WindowStateChanged
    {
        public WindowStateChanged(WindowState windowState)
        {
            WindowState = windowState;
        }

        public WindowState WindowState { get; set; }
    }
}
