namespace HearthCap.Core.GameCapture
{
    using System;
    using System.Threading.Tasks;

    public interface ICaptureEngine
    {
        int Speed { get; set; }

        bool PublishCapturedWindow { get; set; }

        bool IsRunning { get; }

        CaptureMethod CaptureMethod { get; set; }

        Task StartAsync();

        void Stop();

        event EventHandler<UnhandledExceptionEventArgs> UnhandledException;
    }
}