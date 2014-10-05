// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SendNotification.cs" company="">
//   
// </copyright>
// <summary>
//   The send notification.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Notifications
{
    /// <summary>
    /// The send notification.
    /// </summary>
    public class SendNotification
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SendNotification"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="hideAfter">
        /// The hide after.
        /// </param>
        public SendNotification(string message, NotificationType type = NotificationType.Info, int hideAfter = 5000)
        {
            this.Message = message;
            this.Type = type;
            this.HideAfter = hideAfter;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SendNotification"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="hideAfter">
        /// The hide after.
        /// </param>
        public SendNotification(string message, int hideAfter)
            : this(message, NotificationType.Info, hideAfter)
        {
        }

        /// <summary>
        /// Gets or sets the type.
        /// </summary>
        public NotificationType Type { get; set; }

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Hide after (in milliseconds). set to 0 or lower to hide; only on click
        /// </summary>
        public int HideAfter { get; set; }
    }
}