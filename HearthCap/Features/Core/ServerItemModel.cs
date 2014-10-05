// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ServerItemModel.cs" company="">
//   
// </copyright>
// <summary>
//   The server item model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Core
{
    using Caliburn.Micro;

    /// <summary>
    /// The server item model.
    /// </summary>
    public class ServerItemModel : PropertyChangedBase
    {
        /// <summary>
        /// The name.
        /// </summary>
        private string name;

        /// <summary>
        /// The is checked.
        /// </summary>
        private bool isChecked;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServerItemModel"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        public ServerItemModel(string name)
        {
            this.name = name;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name
        {
            get
            {
                return this.name;
            }

            set
            {
                if (value == this.name)
                {
                    return;
                }

                this.name = value;
                this.NotifyOfPropertyChange(() => this.Name);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether is checked.
        /// </summary>
        public bool IsChecked
        {
            get
            {
                return this.isChecked;
            }

            set
            {
                if (value.Equals(this.isChecked))
                {
                    return;
                }

                this.isChecked = value;
                this.NotifyOfPropertyChange(() => this.IsChecked);
            }
        }
    }
}