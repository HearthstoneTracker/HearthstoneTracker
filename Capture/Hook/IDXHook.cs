// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IDXHook.cs" company="">
//   
// </copyright>
// <summary>
//   The DXHook interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Capture.Hook
{
    using System;

    using Capture.Interface;

    /// <summary>
    /// The DXHook interface.
    /// </summary>
    internal interface IDXHook : IDisposable
    {
        #region Public Properties

        /// <summary>
        /// Gets or sets the config.
        /// </summary>
        CaptureConfig Config { get; set; }

        /// <summary>
        /// Gets or sets the interface.
        /// </summary>
        CaptureInterface Interface { get; set; }

        /// <summary>
        /// Gets or sets the request.
        /// </summary>
        ScreenshotRequest Request { get; set; }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// The cleanup.
        /// </summary>
        void Cleanup();

        /// <summary>
        /// The hook.
        /// </summary>
        void Hook();

        #endregion
    }
}