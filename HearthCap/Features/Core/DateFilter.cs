// --------------------------------------------------------------------------------------------------------------------
// <copyright file="DateFilter.cs" company="">
//   
// </copyright>
// <summary>
//   The date filter.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Core
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    using Caliburn.Micro;

    using HearthCap.Util;

    using Action = System.Action;

    /// <summary>
    /// The date filter.
    /// </summary>
    public class DateFilter : PropertyChangedBase
    {
        /// <summary>
        /// The from.
        /// </summary>
        private DateTime? from;

        /// <summary>
        /// The to.
        /// </summary>
        private DateTime? to;

        /// <summary>
        /// The show all time.
        /// </summary>
        private bool showAllTime;

        /// <summary>
        /// The is open.
        /// </summary>
        private bool isOpen;

        /// <summary>
        /// The need refresh.
        /// </summary>
        private bool needRefresh;

        /// <summary>
        /// The date changed.
        /// </summary>
        public event EventHandler DateChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="DateFilter"/> class.
        /// </summary>
        public DateFilter()
        {
            this.PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "From" || args.PropertyName == "To")
                    {
                        this.needRefresh = true;
                        this.OnDateChanged();
                    }
                };
        }

        /// <summary>
        /// Gets or sets a value indicating whether is open.
        /// </summary>
        public bool IsOpen
        {
            get
            {
                return this.isOpen;
            }

            set
            {
                if (value.Equals(this.isOpen))
                {
                    return;
                }

                this.isOpen = value;
                this.NotifyOfPropertyChange(() => this.IsOpen);
            }
        }

        /// <summary>
        /// Gets or sets the from.
        /// </summary>
        public DateTime? From
        {
            get
            {
                return this.from;
            }

            set
            {
                if (value.Equals(this.from))
                {
                    return;
                }

                this.from = value;
                this.NotifyOfPropertyChange(() => this.From);
            }
        }

        /// <summary>
        /// Gets or sets the to.
        /// </summary>
        public DateTime? To
        {
            get
            {
                return this.to;
            }

            set
            {
                if (value.Equals(this.to))
                {
                    return;
                }

                this.to = value;
                this.NotifyOfPropertyChange(() => this.To);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether show all time.
        /// </summary>
        public bool ShowAllTime
        {
            get
            {
                return this.showAllTime;
            }

            set
            {
                if (value.Equals(this.showAllTime))
                {
                    return;
                }

                this.showAllTime = value;
                this.NotifyOfPropertyChange(() => this.ShowAllTime);
            }
        }

        /// <summary>
        /// The set all time.
        /// </summary>
        public void SetAllTime()
        {
            this.From = null;
            this.To = null;
            this.IsOpen = false;
        }

        /// <summary>
        /// The set this year.
        /// </summary>
        public void SetThisYear()
        {
            this.From = new DateTime(DateTime.Now.Year, 1, 1);
            this.To = null;
            this.IsOpen = false;
        }

        /// <summary>
        /// The set this month.
        /// </summary>
        public void SetThisMonth()
        {
            this.From = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.To = null;
            this.IsOpen = false;
        }

        /// <summary>
        /// The set this week.
        /// </summary>
        public void SetThisWeek()
        {
            this.From = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            this.To = null;
            this.IsOpen = false;
        }

        /// <summary>
        /// The set today.
        /// </summary>
        public void SetToday()
        {
            this.From = DateTime.Now.SetToBeginOfDay();
            this.To = null;
            this.IsOpen = false;
        }

        /// <summary>
        /// The on date changed.
        /// </summary>
        protected virtual void OnDateChanged()
        {
            Application.Current.Dispatcher.BeginInvoke(
                (Action)(() =>
                    {
                        if (this.needRefresh)
                        {
                            this.needRefresh = false;
                            var handler = this.DateChanged;
                            if (handler != null)
                            {
                                handler(this, EventArgs.Empty);
                            }
                        }
                    }), DispatcherPriority.ContextIdle);
        }
    }
}