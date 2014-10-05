// --------------------------------------------------------------------------------------------------------------------
// <copyright file="RetrieveImageDataParams.cs" company="">
//   
// </copyright>
// <summary>
//   Used to hold the parameters to be passed to RetrieveImageData
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Hook
{
    using System;

    /// <summary>
    /// Used to hold the parameters to be passed to RetrieveImageData
    /// </summary>
    public struct RetrieveImageDataParams
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the data.
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// Gets or sets the height.
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// Gets or sets the pitch.
        /// </summary>
        public int Pitch { get; set; }

        /// <summary>
        /// Gets or sets the request id.
        /// </summary>
        public Guid RequestId { get; set; }

        /// <summary>
        /// Gets or sets the width.
        /// </summary>
        public int Width { get; set; }

        #endregion
    }
}