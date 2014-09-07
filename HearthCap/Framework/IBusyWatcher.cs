namespace HearthCap.Framework
{
    public interface IBusyWatcher
    {
        bool IsBusy { get; }

        BusyWatcher.BusyWatcherTicket GetTicket();

        void AddWatch();

        void RemoveWatch();
    }
}
