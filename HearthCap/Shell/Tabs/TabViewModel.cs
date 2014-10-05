// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TabViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The tab view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Tabs
{
    using Caliburn.Micro;

    /// <summary>
    /// The tab view model.
    /// </summary>
    public abstract class TabViewModel : Screen, ITab
    {
        /// <summary>
        /// Gets or sets the order.
        /// </summary>
        public int Order { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TabViewModel"/> class.
        /// </summary>
        public TabViewModel()
        {
            this.Order = 1;
        }
    }
}