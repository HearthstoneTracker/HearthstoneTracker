// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IThemeManager.cs" company="">
//   
// </copyright>
// <summary>
//   The ThemeManager interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Theme
{
    using System;
    using System.Collections.Generic;
    using System.Windows;

    using HearthCap.Data;

    using MahApps.Metro;
    using MahApps.Metro.Controls;

    /// <summary>
    /// The ThemeManager interface.
    /// </summary>
    public interface IThemeManager
    {
        /// <summary>
        /// Gets the current configuration.
        /// </summary>
        ThemeConfiguration CurrentConfiguration { get; }

        /// <summary>
        /// Gets the flyout theme.
        /// </summary>
        FlyoutTheme FlyoutTheme { get; }

        /// <summary>
        /// The get theme resources.
        /// </summary>
        /// <returns>
        /// The <see cref="IEnumerable"/>.
        /// </returns>
        IEnumerable<ResourceDictionary> GetThemeResources();

        /// <summary>
        /// The change accent.
        /// </summary>
        /// <param name="accent">
        /// The accent.
        /// </param>
        void ChangeAccent(Accent accent);

        /// <summary>
        /// The apply theme light.
        /// </summary>
        void ApplyThemeLight();

        /// <summary>
        /// The apply theme dark.
        /// </summary>
        void ApplyThemeDark();

        /// <summary>
        /// The apply flyout theme.
        /// </summary>
        /// <param name="theme">
        /// The theme.
        /// </param>
        void ApplyFlyoutTheme(FlyoutTheme theme);

        /// <summary>
        /// The flyout theme changed.
        /// </summary>
        event EventHandler<EventArgs> FlyoutThemeChanged;
    }
}