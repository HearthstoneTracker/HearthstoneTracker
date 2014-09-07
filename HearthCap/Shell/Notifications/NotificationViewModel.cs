namespace HearthCap.Shell.Notifications
{
    using System;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;

    using Caliburn.Micro;

    public class NotificationViewModel : Screen
    {
        private SendNotification notification;

        private bool closing;

        private Duration closeDuration;

        public NotificationViewModel(SendNotification notification)
        {
            this.Notification = notification;
            this.closeDuration = new Duration(TimeSpan.FromSeconds(1));
        }

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

        public void Close()
        {
            base.TryClose();
        }

        public async void CloseAnimated()
        {
            Closing = true;
            await Task.Delay(1000);
            base.TryClose();
        }

        /// <summary>
        /// Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view"/>
        protected override void OnViewLoaded(object view)
        {
            var ui = (Control)view;
            Panel.SetZIndex(ui, int.MaxValue);
        }
    }
}