// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowFound.cs" company="">
//   
// </copyright>
// <summary>
//   The window found.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.EngineEvents
{
    /// <summary>
    /// The window found.
    /// </summary>
    public class WindowFound : EngineEvent
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowFound"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public WindowFound()
            : base("Window found.")
        {
        }
    }
}