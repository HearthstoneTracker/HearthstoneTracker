using System;

namespace HearthCap.Framework
{
    public interface IBusyWatcher
    {
        bool IsBusy { get; }

        IDisposable GetTicket();

        void AddWatch();

        void RemoveWatch();
    }
}
