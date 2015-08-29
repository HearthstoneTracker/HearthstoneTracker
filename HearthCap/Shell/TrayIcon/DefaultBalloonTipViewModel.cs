using System;
using Caliburn.Micro;

namespace HearthCap.Shell.TrayIcon
{
    public class DefaultBalloonTipViewModel : Screen
    {
        private object viewModel;

        private string message;

        private string title;

        public string Title
        {
            get { return title; }
            set
            {
                if (value == title)
                {
                    return;
                }
                title = value;
                NotifyOfPropertyChange(() => Title);
            }
        }

        public string Message
        {
            get { return message; }
            set
            {
                if (value == message)
                {
                    return;
                }
                message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public object ViewModel
        {
            get { return viewModel; }
            set
            {
                if (Equals(value, viewModel))
                {
                    return;
                }
                viewModel = value;
                NotifyOfPropertyChange(() => ViewModel);
            }
        }

        public event EventHandler<EventArgs> BalloonClosing;

        public void OnBalloonClosing()
        {
            var handler = BalloonClosing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}
