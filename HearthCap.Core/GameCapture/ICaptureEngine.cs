using System;
using System.Threading.Tasks;

namespace HearthCap.Core.GameCapture
{
    public interface ICaptureEngine
    {
        int Speed { get; set; }

        bool PublishCapturedWindow { get; set; }

        bool IsRunning { get; }

        CaptureMethod CaptureMethod { get; set; }

        Task StartAsync();

        void Stop();

        event EventHandler<UnhandledExceptionEventArgs> UnhandledException;

        event EventHandler<EventArgs> Started;

        event EventHandler<EventArgs> Stopped;
    }
}
