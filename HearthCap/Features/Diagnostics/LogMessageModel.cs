// --------------------------------------------------------------------------------------------------------------------
// <copyright file="LogMessageModel.cs" company="">
//   
// </copyright>
// <summary>
//   The log message model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Diagnostics
{
    using System;
    using System.Windows;
    using System.Windows.Media;

    using Caliburn.Micro;

    using NLog;

    /// <summary>
    /// The log message model.
    /// </summary>
    public class LogMessageModel : PropertyChangedBase
    {
        /// <summary>
        /// The message.
        /// </summary>
        private string message;

        /// <summary>
        /// The level.
        /// </summary>
        private LogLevel level;

        /// <summary>
        /// The data.
        /// </summary>
        private object data;

        /// <summary>
        /// The date.
        /// </summary>
        private DateTime date;

        /// <summary>
        /// Initializes a new instance of the <see cref="LogMessageModel"/> class.
        /// </summary>
        /// <param name="message">
        /// The message.
        /// </param>
        /// <param name="level">
        /// The level.
        /// </param>
        /// <param name="date">
        /// The date.
        /// </param>
        public LogMessageModel(string message, LogLevel level, DateTime date)
        {
            this.Date = date;
            this.message = message;
            this.level = level;
            this.IsVisible = true;
        }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
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
        /// Gets or sets the level.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the foreground color.
        /// </summary>
        public Brush ForegroundColor { get; set; }

        /// <summary>
        /// Gets or sets the background color.
        /// </summary>
        public Brush BackgroundColor { get; set; }

        /// <summary>
        /// Gets or sets the font style.
        /// </summary>
        public FontStyle FontStyle { get; set; }

        /// <summary>
        /// Gets or sets the font weight.
        /// </summary>
        public FontWeight FontWeight { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is visible.
        /// </summary>
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