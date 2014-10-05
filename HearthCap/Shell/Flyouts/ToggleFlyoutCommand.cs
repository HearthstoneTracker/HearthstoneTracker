// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ToggleFlyoutCommand.cs" company="">
//   
// </copyright>
// <summary>
//   The toggle flyout command.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Flyouts
{
    /// <summary>
    /// The toggle flyout command.
    /// </summary>
    public class ToggleFlyoutCommand
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ToggleFlyoutCommand"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="isModal">
        /// The is modal.
        /// </param>
        public ToggleFlyoutCommand(string name, bool? isModal = null)
        {
            this.Name = name;
            this.IsModal = isModal;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the is modal.
        /// </summary>
        public bool? IsModal { get; set; }

        /// <summary>
        /// Gets or sets the show.
        /// </summary>
        public bool? Show { get; set; }
    }
}