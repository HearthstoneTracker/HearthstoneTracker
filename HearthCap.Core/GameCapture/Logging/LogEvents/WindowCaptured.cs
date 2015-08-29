using System.Drawing;

namespace HearthCap.Core.GameCapture.Logging.LogEvents
{
    public class WindowCaptured : EngineEvent
    {
        public Image Data { get; set; }

        public WindowCaptured(Image data)
            : base("Window captured")
        {
            Data = data;
        }
    }
}
