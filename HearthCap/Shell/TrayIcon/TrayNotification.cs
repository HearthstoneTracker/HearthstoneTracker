namespace HearthCap.Shell.TrayIcon
{
    public class TrayNotification
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TrayNotification(string title, string message, int timeout = 6000)
        {
            Title = title;
            Message = message;
            Timeout = timeout;
        }

        /// <summary>
        ///     Initializes a new instance of the <see cref="T:System.Object" /> class.
        /// </summary>
        public TrayNotification(string title, object viewModel, int timeout = 6000)
        {
            Title = title;
            ViewModel = viewModel;
            Timeout = timeout;
        }

        public string Title { get; set; }

        public object ViewModel { get; set; }

        public string Message { get; set; }

        public int Timeout { get; set; }

        public bool IgnoreShowBalloonSetting { get; set; }

        public string BalloonType { get; set; }
    }
}
