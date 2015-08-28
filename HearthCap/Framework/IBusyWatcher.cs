namespace HearthCap.Framework
{
    using System;

    public interface IBusyWatcher
    {
        bool IsBusy { get; }

        IDisposable GetTicket();

        void AddWatch();

        void RemoveWatch();
    }
}
