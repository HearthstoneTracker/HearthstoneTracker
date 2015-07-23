namespace HearthCap.Shell.Events
{
    using System.Windows;

    public class WindowStateChanged
    {
        public WindowStateChanged(WindowState windowState)
        {
            this.WindowState = windowState;
        }

        public WindowState WindowState { get; set; }
    }
}