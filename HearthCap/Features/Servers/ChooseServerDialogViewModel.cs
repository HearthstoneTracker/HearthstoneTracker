using System.Linq;
using Caliburn.Micro;
using Caliburn.Micro.Recipes.Filters;
using HearthCap.Features.Core;

namespace HearthCap.Features.Servers
{
    public class ChooseServerDialogViewModel : Screen
    {
        private readonly BindableCollection<ServerItemModel> servers;

        private ServerItemModel selectedServer;

        public ChooseServerDialogViewModel(BindableCollection<ServerItemModel> servers)
        {
            this.servers = servers;
            DisplayName = "Choose default server:";
            SelectedServer = servers.FirstOrDefault(x => x.IsChecked);
            if (SelectedServer == null)
            {
                SelectedServer = servers.FirstOrDefault();
            }
        }

        public BindableCollection<ServerItemModel> Servers
        {
            get { return servers; }
        }

        public ServerItemModel SelectedServer
        {
            get { return selectedServer; }
            set
            {
                if (Equals(value, selectedServer))
                {
                    return;
                }
                selectedServer = value;
                NotifyOfPropertyChange(() => SelectedServer);
            }
        }

        [Dependencies("SelectedServer")]
        public void SaveClose()
        {
            TryClose();
        }

        public bool CanSaveClose()
        {
            return SelectedServer != null;
        }
    }
}
