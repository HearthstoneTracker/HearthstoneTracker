using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using Caliburn.Micro;

namespace HearthCap.Shell.Notifications
{
    public class NotificationViewModel : Screen
    {
        private SendNotification notification;

        private bool closing;

        private Duration closeDuration;

        public NotificationViewModel(SendNotification notification)
        {
            Notification = notification;
            closeDuration = new Duration(TimeSpan.FromSeconds(1));
        }

        public SendNotification Notification
        {
            get { return notification; }
            set
            {
                if (Equals(value, notification))
                {
                    return;
                }
                notification = value;
                NotifyOfPropertyChange(() => Notification);
            }
        }

        public bool Closing
        {
            get { return closing; }
            set
            {
                if (value.Equals(closing))
                {
                    return;
                }
                closing = value;
                NotifyOfPropertyChange(() => Closing);
            }
        }

        public Duration CloseDuration
        {
            get { return closeDuration; }
            set
            {
                if (value.Equals(closeDuration))
                {
                    return;
                }
                closeDuration = value;
                NotifyOfPropertyChange(() => CloseDuration);
            }
        }

        public void Close()
        {
            TryClose();
        }

        public async void CloseAnimated()
        {
            Closing = true;
            await Task.Delay(1000);
            TryClose();
        }

        /// <summary>
        ///     Called when an attached view's Loaded event fires.
        /// </summary>
        /// <param name="view" />
        protected override void OnViewLoaded(object view)
        {
            var ui = (Control)view;
            Panel.SetZIndex(ui, int.MaxValue);
        }
    }
}
