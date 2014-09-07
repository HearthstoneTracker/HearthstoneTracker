namespace HearthCap.Core.GameCapture.Logging.LogEvents
{
    using System.Drawing;

    public class WindowCaptured : EngineEvent
    {
        public Image Data { get; set; }

        public WindowCaptured(Image data)
            : base("Window captured")
        {
            this.Data = data;
        }
    }
}