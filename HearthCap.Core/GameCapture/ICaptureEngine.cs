// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICaptureEngine.cs" company="">
//   
// </copyright>
// <summary>
//   The CaptureEngine interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture
{
    using System;
    using System.Threading.Tasks;

    /// <summary>
    /// The CaptureEngine interface.
    /// </summary>
    public interface ICaptureEngine
    {
        /// <summary>
        /// Gets or sets the speed.
        /// </summary>
        int Speed { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether publish captured window.
        /// </summary>
        bool PublishCapturedWindow { get; set; }

        /// <summary>
        /// Gets a value indicating whether is running.
        /// </summary>
        bool IsRunning { get; }

        /// <summary>
        /// Gets or sets the capture method.
        /// </summary>
        CaptureMethod CaptureMethod { get; set; }

        /// <summary>
        /// The start async.
        /// </summary>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        Task StartAsync();

        /// <summary>
        /// The stop.
        /// </summary>
        void Stop();

        /// <summary>
        /// The unhandled exception.
        /// </summary>
        event EventHandler<UnhandledExceptionEventArgs> UnhandledException;
    }
}