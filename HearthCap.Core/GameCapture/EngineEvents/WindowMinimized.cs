// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowMinimized.cs" company="">
//   
// </copyright>
// <summary>
//   The window minimized.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.EngineEvents
{
    /// <summary>
    /// The window minimized.
    /// </summary>
    public class WindowMinimized : EngineEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowMinimized"/> class.
        /// </summary>
        public WindowMinimized()
            : base("Window is minimized")
        {
        }
    }
}