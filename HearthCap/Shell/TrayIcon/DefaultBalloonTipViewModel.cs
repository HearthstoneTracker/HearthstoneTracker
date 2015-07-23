namespace HearthCap.Shell.TrayIcon
{
    using System;

    using Caliburn.Micro;

    public class DefaultBalloonTipViewModel : Screen
    {
        private object viewModel;

        private string message;

        private string title;

        public string Title
        {
            get
            {
                return this.title;
            }
            set
            {
                if (value == this.title)
                {
                    return;
                }
                this.title = value;
                this.NotifyOfPropertyChange(() => this.Title);
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }
            set
            {
                if (value == this.message)
                {
                    return;
                }
                this.message = value;
                this.NotifyOfPropertyChange(() => this.Message);
            }
        }

        public object ViewModel
        {
            get
            {
                return this.viewModel;
            }
            set
            {
                if (Equals(value, this.viewModel))
                {
                    return;
                }
                this.viewModel = value;
                this.NotifyOfPropertyChange(() => this.ViewModel);
            }
        }

        public event EventHandler<EventArgs> BalloonClosing;

        public void OnBalloonClosing()
        {
            var handler = this.BalloonClosing;
            if (handler != null)
            {
                handler(this, EventArgs.Empty);
            }
        }
    }
}