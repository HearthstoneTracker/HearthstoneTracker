namespace HearthCap.Features.Core
{
    using System;
    using System.Windows;
    using System.Windows.Threading;

    using Caliburn.Micro;

    using HearthCap.Util;

    using Action = System.Action;

    public class DateFilter : PropertyChangedBase
    {
        private DateTime? from;

        private DateTime? to;

        private bool showAllTime;

        private bool isOpen;

        private bool needRefresh;

        public event EventHandler DateChanged;

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

        public void SetAllTime()
        {
            this.From = null;
            this.To = null;
            this.IsOpen = false;
        }

        public void SetThisYear()
        {
            this.From = new DateTime(DateTime.Now.Year, 1, 1);
            this.To = null;
            this.IsOpen = false;
        }

        public void SetThisMonth()
        {
            this.From = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            this.To = null;
            this.IsOpen = false;
        }

        public void SetThisWeek()
        {
            this.From = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            this.To = null;
            this.IsOpen = false;
        }

        public void SetToday()
        {
            this.From = DateTime.Now.SetToBeginOfDay();
            this.To = null;
            this.IsOpen = false;
        }

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