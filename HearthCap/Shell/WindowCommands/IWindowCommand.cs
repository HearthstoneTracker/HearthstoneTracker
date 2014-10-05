// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWindowCommand.cs" company="">
//   
// </copyright>
// <summary>
//   The WindowCommand interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.WindowCommands
{
    using Caliburn.Micro;

    /// <summary>
    /// The WindowCommand interface.
    /// </summary>
    public interface IWindowCommand : INotifyPropertyChangedEx
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        int Order { get; set; }
    }
}