using System;

namespace HearthCap.Core.GameCapture
{
    public class EngineSwitchEventArgs : EventArgs
    {
        public bool? FullscreenSupport { get; set; }

        public bool? BackgroundCaptureSupport { get; set; }
    }
}
