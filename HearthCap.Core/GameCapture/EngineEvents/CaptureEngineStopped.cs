namespace HearthCap.Core.GameCapture.EngineEvents
{
    public class CaptureEngineStopped : EngineEvent
    {
        public CaptureEngineStopped()
            : base("Engine stopped")
        {
        }

        public string LastErrorMessage { get; set; }
    }
}