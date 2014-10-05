// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingsScreen.cs" company="">
//   
// </copyright>
// <summary>
//   The settings screen.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Settings
{
    using Caliburn.Micro;

    /// <summary>
    /// The settings screen.
    /// </summary>
    public abstract class SettingsScreen : Screen, ISettingsScreen
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }
    }
}