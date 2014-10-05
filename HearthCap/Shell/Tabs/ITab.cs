// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ITab.cs" company="">
//   
// </copyright>
// <summary>
//   The Tab interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Tabs
{
    using Caliburn.Micro;

    /// <summary>
    /// The Tab interface.
    /// </summary>
    public interface ITab : IScreen
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        int Order { get; set; }
    }
}