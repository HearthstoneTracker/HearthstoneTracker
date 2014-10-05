// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CaptureConfig.cs" company="">
//   
// </copyright>
// <summary>
//   The capture config.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Interface
{
    using System;

    /// <summary>
    /// The capture config.
    /// </summary>
    [Serializable]
    public class CaptureConfig
    {
        #region Constructors and Destructors

        /// <summary>
        /// Initializes a new instance of the <see cref="CaptureConfig"/> class.
        /// </summary>
        public CaptureConfig()
        {
            this.Direct3DVersion = Direct3DVersion.AutoDetect;
        }

        #endregion

        #region Public Properties

        /// <summary>
        /// Gets or sets the direct 3 d version.
        /// </summary>
        public Direct3DVersion Direct3DVersion { get; set; }

        #endregion
    }
}