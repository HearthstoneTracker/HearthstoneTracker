namespace HearthCap.Shell.TrayIcon
{
    using System.Collections.Generic;

    public class TrayNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TrayNotification(string title, string message, int timeout = 6000)
        {
            this.Title = title;
            this.Message = message;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public TrayNotification(string title, object viewModel, int timeout = 6000)
        {
            this.Title = title;
            this.ViewModel = viewModel;
            this.Timeout = timeout;
        }

        public string Title { get; set; }

        public object ViewModel { get; set; }

        public string Message { get; set; }

        public int Timeout { get; set; }

        public bool IgnoreShowBalloonSetting { get; set; }

        public string BalloonType { get; set; }
    }
}