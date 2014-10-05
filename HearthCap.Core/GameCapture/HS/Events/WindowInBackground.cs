// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowInBackground.cs" company="">
//   
// </copyright>
// <summary>
//   The window in background.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.HS.Events
{
    /// <summary>
    /// The window in background.
    /// </summary>
    public class WindowInBackground : GameEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowInBackground"/> class.
        /// </summary>
        public WindowInBackground()
            : base("Window is probably in background")
        {
        }
    }
}