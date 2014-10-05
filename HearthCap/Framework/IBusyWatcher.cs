// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBusyWatcher.cs" company="">
//   
// </copyright>
// <summary>
//   The BusyWatcher interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Framework
{
    /// <summary>
    /// The BusyWatcher interface.
    /// </summary>
    public interface IBusyWatcher
    {
        /// <summary>
        /// Gets a value indicating whether is busy.
        /// </summary>
        bool IsBusy { get; }

        /// <summary>
        /// The get ticket.
        /// </summary>
        /// <returns>
        /// The <see cref="BusyWatcherTicket"/>.
        /// </returns>
        BusyWatcher.BusyWatcherTicket GetTicket();

        /// <summary>
        /// The add watch.
        /// </summary>
        void AddWatch();

        /// <summary>
        /// The remove watch.
        /// </summary>
        void RemoveWatch();
    }
}
