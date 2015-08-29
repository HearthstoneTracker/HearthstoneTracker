using System;
using System.Windows;
using System.Windows.Threading;
using Caliburn.Micro;
using HearthCap.Util;
using Action = System.Action;

namespace HearthCap.Features.Core
{
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
            PropertyChanged += (sender, args) =>
                {
                    if (args.PropertyName == "From"
                        || args.PropertyName == "To")
                    {
                        needRefresh = true;
                        OnDateChanged();
                    }
                };
        }

        public bool IsOpen
        {
            get { return isOpen; }
            set
            {
                if (value.Equals(isOpen))
                {
                    return;
                }
                isOpen = value;
                NotifyOfPropertyChange(() => IsOpen);
            }
        }

        public DateTime? From
        {
            get { return from; }
            set
            {
                if (value.Equals(from))
                {
                    return;
                }
                from = value;
                NotifyOfPropertyChange(() => From);
            }
        }

        public DateTime? To
        {
            get { return to; }
            set
            {
                if (value.Equals(to))
                {
                    return;
                }
                to = value;
                NotifyOfPropertyChange(() => To);
            }
        }

        public bool ShowAllTime
        {
            get { return showAllTime; }
            set
            {
                if (value.Equals(showAllTime))
                {
                    return;
                }
                showAllTime = value;
                NotifyOfPropertyChange(() => ShowAllTime);
            }
        }

        public void SetAllTime()
        {
            From = null;
            To = null;
            IsOpen = false;
        }

        public void SetThisYear()
        {
            From = new DateTime(DateTime.Now.Year, 1, 1);
            To = null;
            IsOpen = false;
        }

        public void SetThisMonth()
        {
            From = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            To = null;
            IsOpen = false;
        }

        public void SetThisWeek()
        {
            From = DateTime.Now.StartOfWeek(DayOfWeek.Monday);
            To = null;
            IsOpen = false;
        }

        public void SetToday()
        {
            From = DateTime.Now.SetToBeginOfDay();
            To = null;
            IsOpen = false;
        }

        protected virtual void OnDateChanged()
        {
            Application.Current.Dispatcher.BeginInvoke(
                (Action)(() =>
                    {
                        if (needRefresh)
                        {
                            needRefresh = false;
                            var handler = DateChanged;
                            if (handler != null)
                            {
                                handler(this, EventArgs.Empty);
                            }
                        }
                    }), DispatcherPriority.ContextIdle);
        }
    }
}
