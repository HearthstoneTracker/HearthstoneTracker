// --------------------------------------------------------------------------------------------------------------------
// <copyright file="EngineSwitchEventArgs.cs" company="">
//   
// </copyright>
// <summary>
//   The engine switch event args.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture
{
    using System;

    /// <summary>
    /// The engine switch event args.
    /// </summary>
    public class EngineSwitchEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the fullscreen support.
        /// </summary>
        public bool? FullscreenSupport { get; set; }

        /// <summary>
        /// Gets or sets the background capture support.
        /// </summary>
        public bool? BackgroundCaptureSupport { get; set; }
    }
}