using System;
using System.ComponentModel.Composition;
using System.Threading;
using Caliburn.Micro;

namespace HearthCap.Framework
{
    [Export(typeof(IBusyWatcher))]
    public class BusyWatcher : PropertyChangedBase, IBusyWatcher
    {
        private int counter;

        public bool IsBusy
        {
            get { return counter > 0; }
        }

        public IDisposable GetTicket()
        {
            return new BusyWatcherTicket(this);
        }

        public void AddWatch()
        {
            if (Interlocked.Increment(ref counter) == 1)
            {
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        public void RemoveWatch()
        {
            if (Interlocked.Decrement(ref counter) == 0)
            {
                NotifyOfPropertyChange(() => IsBusy);
            }
        }

        private sealed class BusyWatcherTicket : IDisposable
        {
            private readonly IBusyWatcher _parent;

            public BusyWatcherTicket(IBusyWatcher parent)
            {
                _parent = parent;
                _parent.AddWatch();
            }

            public void Dispose()
            {
                _parent.RemoveWatch();
            }
        }
    }
}
