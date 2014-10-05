// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IFlyout.cs" company="">
//   
// </copyright>
// <summary>
//   The Flyout interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Flyouts
{
    using Caliburn.Micro;

    using MahApps.Metro.Controls;

    /// <summary>
    /// The Flyout interface.
    /// </summary>
    public interface IFlyout : IScreen
    {
        /// <summary>
        /// Gets or sets the header.
        /// </summary>
        string Header { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is open.
        /// </summary>
        bool IsOpen { get; set; }

        /// <summary>
        /// Gets or sets the position.
        /// </summary>
        Position Position { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        string Name { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is modal.
        /// </summary>
        bool IsModal { get; set; }

        /// <summary>
        /// Gets the theme.
        /// </summary>
        FlyoutTheme Theme { get; }
    }
}