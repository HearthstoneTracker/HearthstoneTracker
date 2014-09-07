namespace HearthCap.Framework
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading;

    using Caliburn.Micro;

    [Export(typeof(IBusyWatcher))]
    public class BusyWatcher : PropertyChangedBase, IBusyWatcher
    {
        private int counter;

        public bool IsBusy
        {
            get
            {
                return this.counter > 0;
            }
        }

        public BusyWatcherTicket GetTicket()
        {
            return new BusyWatcherTicket(this);
        }

        public void AddWatch()
        {
            if (Interlocked.Increment(ref this.counter) == 1)
            {
                this.NotifyOfPropertyChange(() => this.IsBusy);
            }
        }

        public void RemoveWatch()
        {
            if (Interlocked.Decrement(ref this.counter) == 0)
            {
                this.NotifyOfPropertyChange(() => this.IsBusy);
            }
        }

        public class BusyWatcherTicket : IDisposable
        {
            private readonly IBusyWatcher parent;

            public BusyWatcherTicket(IBusyWatcher parent)
            {
                this.parent = parent;
                this.parent.AddWatch();
            }

            public void Dispose()
            {
                this.parent.RemoveWatch();
            }
        }
    }
}
