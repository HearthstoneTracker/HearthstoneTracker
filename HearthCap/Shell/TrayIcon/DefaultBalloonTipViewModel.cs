// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DefaultBalloonTipViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The default balloon tip view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.TrayIcon
{
    using System;

    using Caliburn.Micro;

    /// <summary>
    /// The default balloon tip view model.
    /// </summary>
    public class DefaultBalloonTipViewModel : Screen
    {
        /// <summary>
        /// The view model.
        /// </summary>
        private object viewModel;

        /// <summary>
        /// The message.
        /// </summary>
        private string message;

        /// <summary>
        /// The title.
        /// </summary>
        private string title;

        /// <summary>
        /// Gets or sets the title.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the message.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the view model.
        /// </summary>
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

        /// <summary>
        /// The balloon closing.
        /// </summary>
        public event EventHandler<EventArgs> BalloonClosing;

        /// <summary>
        /// The on balloon closing.
        /// </summary>
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