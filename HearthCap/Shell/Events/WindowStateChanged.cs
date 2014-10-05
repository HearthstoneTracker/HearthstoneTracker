// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowStateChanged.cs" company="">
//   
// </copyright>
// <summary>
//   The window state changed.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Events
{
    using System.Windows;

    /// <summary>
    /// The window state changed.
    /// </summary>
    public class WindowStateChanged
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowStateChanged"/> class.
        /// </summary>
        /// <param name="windowState">
        /// The window state.
        /// </param>
        public WindowStateChanged(WindowState windowState)
        {
            this.WindowState = windowState;
        }

        /// <summary>
        /// Gets or sets the window state.
        /// </summary>
        public WindowState WindowState { get; set; }
    }
}