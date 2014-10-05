// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ChooseServerDialogViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The choose server dialog view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Servers
{
    using System.Linq;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters;

    using HearthCap.Features.Core;

    /// <summary>
    /// The choose server dialog view model.
    /// </summary>
    public class ChooseServerDialogViewModel : Screen
    {
        /// <summary>
        /// The servers.
        /// </summary>
        private readonly BindableCollection<ServerItemModel> servers;

        /// <summary>
        /// The selected server.
        /// </summary>
        private ServerItemModel selectedServer;

        /// <summary>
        /// Initializes a new instance of the <see cref="ChooseServerDialogViewModel"/> class.
        /// </summary>
        /// <param name="servers">
        /// The servers.
        /// </param>
        public ChooseServerDialogViewModel(BindableCollection<ServerItemModel> servers)
        {
            this.servers = servers;
            this.DisplayName = "Choose default server:";
            this.SelectedServer = servers.FirstOrDefault(x => x.IsChecked);
            if (this.SelectedServer == null)
            {
                this.SelectedServer = servers.FirstOrDefault();
            }
        }

        /// <summary>
        /// Gets the servers.
        /// </summary>
        public BindableCollection<ServerItemModel> Servers
        {
            get
            {
                return this.servers;
            }
        }

        /// <summary>
        /// Gets or sets the selected server.
        /// </summary>
        public ServerItemModel SelectedServer
        {
            get
            {
                return this.selectedServer;
            }

            set
            {
                if (Equals(value, this.selectedServer))
                {
                    return;
                }

                this.selectedServer = value;
                this.NotifyOfPropertyChange(() => this.SelectedServer);
            }
        }

        /// <summary>
        /// The save close.
        /// </summary>
        [Dependencies("SelectedServer")]
        public void SaveClose()
        {
            this.TryClose();
        }

        /// <summary>
        /// The can save close.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanSaveClose()
        {
            return this.SelectedServer != null;
        }
    }
}