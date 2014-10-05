// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowCaptured.cs" company="">
//   
// </copyright>
// <summary>
//   The window captured.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.Logging.LogEvents
{
    using System.Drawing;

    /// <summary>
    /// The window captured.
    /// </summary>
    public class WindowCaptured : EngineEvent
    {
        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public Image Data { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCaptured"/> class.
        /// </summary>
        /// <param name="data">
        /// The data.
        /// </param>
        public WindowCaptured(Image data)
            : base("Window captured")
        {
            this.Data = data;
        }
    }
}