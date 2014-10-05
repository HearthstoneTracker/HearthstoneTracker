// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICommandBarItem.cs" company="">
//   
// </copyright>
// <summary>
//   The CommandBarItem interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.CommandBar
{
    /// <summary>
    /// The CommandBarItem interface.
    /// </summary>
    public interface ICommandBarItem
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        int Order { get; set; }
    }
}