namespace HearthCap.Logging
{
    using System;

    public interface IAppLogManager : IDisposable
    {
        void Flush();

        void Initialize(string logFilesDirectory);
    }
}