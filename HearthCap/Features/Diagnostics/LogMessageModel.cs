namespace HearthCap.Features.Diagnostics
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    using Caliburn.Micro;
    using NLog;

    public class LogMessageModel : PropertyChangedBase
    {
        private string _message;

        private LogLevel _level;

        private object _data;

        private DateTime _date;

        public LogMessageModel(string message, LogLevel level, DateTime date)
        {
            _date = date;
            _message = message;
            _level = level;
            IsVisible = true;
        }

        public DateTime Date
        {
            get
            {
                return _date;
            }
            set
            {
                if (value.Equals(_date))
                {
                    return;
                }
                _date = value;
                NotifyOfPropertyChange(() => Date);
            }
        }

        public string Message
        {
            get
            {
                return _message;
            }
            set
            {
                if (value == _message)
                {
                    return;
                }
                _message = value;
                NotifyOfPropertyChange(() => Message);
            }
        }

        public LogLevel Level
        {
            get
            {
                return _level;
            }
            set
            {
                if (value == _level)
                {
                    return;
                }
                _level = value;
                NotifyOfPropertyChange(() => Level);
            }
        }

        public object Data
        {
            get
            {
                return _data;
            }
            set
            {
                if (Equals(value, _data))
                {
                    return;
                }
                _data = value;
                NotifyOfPropertyChange(() => Data);
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
            return Message;
        }
    }
}