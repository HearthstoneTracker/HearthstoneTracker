// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IViewLocator.cs" company="">
//   
// </copyright>
// <summary>
//   The ViewLocator interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Theme
{
    using System;
    using System.Windows;

    /// <summary>
    /// The ViewLocator interface.
    /// </summary>
    public interface IViewLocator
    {
        /// <summary>
        /// The get or create view type.
        /// </summary>
        /// <param name="viewType">
        /// The view type.
        /// </param>
        /// <returns>
        /// The <see cref="UIElement"/>.
        /// </returns>
        UIElement GetOrCreateViewType(Type viewType);
    }
}