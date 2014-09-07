namespace Capture.Hook
{
    using System;

    using Capture.Interface;

    internal interface IDXHook : IDisposable
    {
        #region Public Properties

        CaptureConfig Config { get; set; }

        CaptureInterface Interface { get; set; }

        ScreenshotRequest Request { get; set; }

        #endregion

        #region Public Methods and Operators

        void Cleanup();

        void Hook();

        #endregion
    }
}