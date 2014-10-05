// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowNotFound.cs" company="">
//   
// </copyright>
// <summary>
//   The window not found.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.EngineEvents
{
    /// <summary>
    /// The window not found.
    /// </summary>
    public class WindowNotFound : EngineEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowNotFound"/> class.
        /// </summary>
        public WindowNotFound()
            : base("Window not found")
        {
        }
    }
}