namespace HearthCap.Shell.Notifications
{
    public class SendNotification
    {
        public SendNotification(string message, NotificationType type = NotificationType.Info, int hideAfter = 5000)
        {
            this.Message = message;
            this.Type = type;
            this.HideAfter = hideAfter;
        }

        public SendNotification(string message, int hideAfter)
            : this(message, NotificationType.Info, hideAfter)
        {
        }

        public NotificationType Type { get; set; }

        public string Message { get; set; }

        /// <summary>
        /// Hide after (in milliseconds). set to 0 or lower to hide; only on click
        /// </summary>
        public int HideAfter { get; set; }
    }
}