// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NotificationViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The notification view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Notifications
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    /// <summary>
    /// The notification view model.
    /// </summary>
    public class NotificationViewModel : Screen
    {
        /// <summary>
        /// The notification.
        /// </summary>
        private SendNotification notification;

        /// <summary>
        /// The closing.
        /// </summary>
        private bool closing;

        /// <summary>
        /// The close duration.
        /// </summary>
        private Duration closeDuration;

        /// <summary>
        /// Initializes a new instance of the <see cref="NotificationViewModel"/> class.
        /// </summary>
        /// <param name="notification">
        /// The notification.
        /// </param>
        public NotificationViewModel(SendNotification notification)
        {
            this.Notification = notification;
            this.closeDuration = new Duration(TimeSpan.FromSeconds(1));
        }

        /// <summary>
        /// Gets or sets the notification.
        /// </summary>
        public SendNotification Notification
        {
            get
            {
                return this.notification;
            }

            set
            {
                if (Equals(value, this.notification))
                {
                    return;
                }

                this.notification = value;
                this.NotifyOfPropertyChange(() => this.Notification);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether closing.
        /// </summary>
        public bool Closing
        {
            get
            {
                return this.closing;
            }

            set
            {
                if (value.Equals(this.closing))
                {
                    return;
                }

                this.closing = value;
                this.NotifyOfPropertyChange(() => this.Closing);
            }
        }

        /// <summary>
        /// Gets or sets the close duration.
        /// </summary>
        public Duration CloseDuration
        {
            get
            {
                return this.closeDuration;
            }

            set
            {
                if (value.Equals(this.closeDuration))
                {
                    return;
                }

                this.closeDuration = value;
                this.NotifyOfPropertyChange(() => this.CloseDuration);
            }
        }

        /// <summary>
        /// The close.
        /// </summary>
        public void Close()
        {
            this.TryClose();
        }

        /// <summary>
        /// The close animated.
        /// </summary>
        public async void CloseAnimated()
        {
            this.Closing = true;
            await Task.Delay(1000);
            this.TryClose();
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view">
        /// </param>
        protected override void OnViewLoaded(object view)
        {
            var ui = (Control)view;
            Panel.SetZIndex(ui, int.MaxValue);
        }
    }
}