// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WindowCommandViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The window command view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.WindowCommands
{
    using Caliburn.Micro;

    /// <summary>
    /// The window command view model.
    /// </summary>
    public abstract class WindowCommandViewModel : PropertyChangedBase, IWindowCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="WindowCommandViewModel"/> class. 
        /// Creates an instance of <see cref="T:Caliburn.Micro.PropertyChangedBase"/>.
        /// </summary>
        protected WindowCommandViewModel()
        {
            this.Order = 1;
        }

        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }
    }
}