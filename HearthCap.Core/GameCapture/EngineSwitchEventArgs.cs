namespace HearthCap.Core.GameCapture
{
    using System;

    public class EngineSwitchEventArgs : EventArgs
    {
        public bool? FullscreenSupport { get; set; }

        public bool? BackgroundCaptureSupport { get; set; }
    }
}