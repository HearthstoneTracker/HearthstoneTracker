// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureEngineStopped.cs" company="">
//   
// </copyright>
// <summary>
//   The capture engine stopped.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.EngineEvents
{
    /// <summary>
    /// The capture engine stopped.
    /// </summary>
    public class CaptureEngineStopped : EngineEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureEngineStopped"/> class.
        /// </summary>
        public CaptureEngineStopped()
            : base("Engine stopped")
        {
        }

        /// <summary>
        /// Gets or sets the last error message.
        /// </summary>
        public string LastErrorMessage { get; set; }
    }
}