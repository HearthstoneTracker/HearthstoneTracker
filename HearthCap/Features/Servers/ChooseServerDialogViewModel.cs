namespace HearthCap.Features.Servers
{
    using System.Linq;

    using Caliburn.Micro;
    using Caliburn.Micro.Recipes.Filters;

    using HearthCap.Features.Core;

    public class ChooseServerDialogViewModel : Screen
    {
        private readonly BindableCollection<ServerItemModel> servers;

        private ServerItemModel selectedServer;

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

        public BindableCollection<ServerItemModel> Servers
        {
            get
            {
                return this.servers;
            }
        }

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

        [Dependencies("SelectedServer")]
        public void SaveClose()
        {
            this.TryClose();
        }

        public bool CanSaveClose()
        {
            return this.SelectedServer != null;
        }
    }
}