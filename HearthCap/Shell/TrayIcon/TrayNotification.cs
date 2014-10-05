// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TrayNotification.cs" company="">
//   
// </copyright>
// <summary>
//   The tray notification.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.TrayIcon
{
    /// <summary>
    /// The tray notification.
    /// </summary>
    public class TrayNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TrayNotification"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        public TrayNotification(string title, string message, int timeout = 6000)
        {
            this.Title = title;
            this.Message = message;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TrayNotification"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="title">
        /// The title.
        /// </param>
        /// <param name="viewModel">
        /// The view Model.
        /// </param>
        /// <param name="timeout">
        /// The timeout.
        /// </param>
        public TrayNotification(string title, object viewModel, int timeout = 6000)
        {
            this.Title = title;
            this.ViewModel = viewModel;
            this.Timeout = timeout;
        }

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
        public object ViewModel { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Gets or sets the timeout.
        /// </summary>
        public int Timeout { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether ignore show balloon setting.
        /// </summary>
        public bool IgnoreShowBalloonSetting { get; set; }

        /// <summary>
        /// Gets or sets the balloon type.
        /// </summary>
        public string BalloonType { get; set; }
    }
}