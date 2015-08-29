using System;

namespace HearthCap.Logging
{
    public interface IAppLogManager : IDisposable
    {
        void Flush();

        void Initialize(string logFilesDirectory);
    }
}
