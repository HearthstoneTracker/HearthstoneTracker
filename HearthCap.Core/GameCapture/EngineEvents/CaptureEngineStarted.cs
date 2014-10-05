// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureEngineStarted.cs" company="">
//   
// </copyright>
// <summary>
//   The capture engine started.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.EngineEvents
{
    /// <summary>
    /// The capture engine started.
    /// </summary>
    public class CaptureEngineStarted : EngineEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureEngineStarted"/> class.
        /// </summary>
        public CaptureEngineStarted()
            : base("Engine started")
        {
        }
    }
}