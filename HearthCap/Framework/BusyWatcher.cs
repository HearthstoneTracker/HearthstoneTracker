// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BusyWatcher.cs" company="">
//   
// </copyright>
// <summary>
//   The busy watcher.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Framework
{
    using System;
    using System.ComponentModel.Composition;
    using System.Threading;

    using Caliburn.Micro;

    /// <summary>
    /// The busy watcher.
    /// </summary>
    [Export(typeof(IBusyWatcher))]
    public class BusyWatcher : PropertyChangedBase, IBusyWatcher
    {
        /// <summary>
        /// The counter.
        /// </summary>
        private int counter;

        /// <summary>
        /// Gets a value indicating whether is busy.
        /// </summary>
        public bool IsBusy
        {
            get
            {
                return this.counter > 0;
            }
        }

        /// <summary>
        /// The get ticket.
        /// </summary>
        /// <returns>
        /// The <see cref="BusyWatcherTicket"/>.
        /// </returns>
        public BusyWatcherTicket GetTicket()
        {
            return new BusyWatcherTicket(this);
        }

        /// <summary>
        /// The add watch.
        /// </summary>
        public void AddWatch()
        {
            if (Interlocked.Increment(ref this.counter) == 1)
            {
                this.NotifyOfPropertyChange(() => this.IsBusy);
            }
        }

        /// <summary>
        /// The remove watch.
        /// </summary>
        public void RemoveWatch()
        {
            if (Interlocked.Decrement(ref this.counter) == 0)
            {
                this.NotifyOfPropertyChange(() => this.IsBusy);
            }
        }

        /// <summary>
        /// The busy watcher ticket.
        /// </summary>
        public class BusyWatcherTicket : IDisposable
        {
            /// <summary>
            /// The parent.
            /// </summary>
            private readonly IBusyWatcher parent;

            /// <summary>
            /// Initializes a new instance of the <see cref="BusyWatcherTicket"/> class.
            /// </summary>
            /// <param name="parent">
            /// The parent.
            /// </param>
            public BusyWatcherTicket(IBusyWatcher parent)
            {
                this.parent = parent;
                this.parent.AddWatch();
            }

            /// <summary>
            /// The dispose.
            /// </summary>
            public void Dispose()
            {
                this.parent.RemoveWatch();
            }
        }
    }
}
