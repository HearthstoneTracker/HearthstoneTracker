namespace HearthCap.Features.Diagnostics
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    using Caliburn.Micro;
    using NLog;

    public class LogMessageModel : PropertyChangedBase
    {
        private string message;

        private LogLevel level;

        private object data;

        private DateTime date;

        public LogMessageModel(string message, LogLevel level, DateTime date)
        {
            this.Date = date;
            this.message = message;
            this.level = level;
            IsVisible = true;
        }

        public DateTime Date
        {
            get
            {
                return this.date;
            }
            set
            {
                if (value.Equals(this.date))
                {
                    return;
                }
                this.date = value;
                this.NotifyOfPropertyChange(() => this.Date);
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

        public LogLevel Level
        {
            get
            {
                return this.level;
            }
            set
            {
                if (value == this.level)
                {
                    return;
                }
                this.level = value;
                this.NotifyOfPropertyChange(() => this.Level);
            }
        }

        public object Data
        {
            get
            {
                return this.data;
            }
            set
            {
                if (Equals(value, this.data))
                {
                    return;
                }
                this.data = value;
                this.NotifyOfPropertyChange(() => this.Data);
            }
        }

        public Brush ForegroundColor { get; set; }

        public Brush BackgroundColor { get; set; }

        public FontStyle FontStyle { get; set; }

        public FontWeight FontWeight { get; set; }

        public bool IsVisible { get; set; }

        /// <summary>
        /// Returns a string that represents the current object.
        /// </summary>
        /// <returns>
        /// A string that represents the current object.
        /// </returns>
        public override string ToString()
        {
            return this.Message;
        }
    }
}