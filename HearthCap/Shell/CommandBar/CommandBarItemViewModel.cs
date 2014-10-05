// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CommandBarItemViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The command bar item view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.CommandBar
{
    using Caliburn.Micro;

    /// <summary>
    /// The command bar item view model.
    /// </summary>
    public class CommandBarItemViewModel : PropertyChangedBase, ICommandBarItem
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }
    }
}