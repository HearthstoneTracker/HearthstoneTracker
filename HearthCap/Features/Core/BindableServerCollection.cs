namespace HearthCap.Features.Core
{
    using System;

    using Caliburn.Micro;

    using HearthCap.Shell.UserPreferences;

    public class BindableServerCollection : BindableCollection<ServerItemModel>
    {
        private IEventAggregator events;

        private readonly static BindableServerCollection instance = new BindableServerCollection();

        private ServerItemModel @default;

        private BindableServerCollection()
        {
            events = IoC.Get<IEventAggregator>();

            using (var settings = new ApplicationRegistrySettings())
            {
                var serverlist = settings.Servers.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in serverlist)
                {
                    var item = new ServerItemModel(name);
                    Add(item);
                    if (settings.DefaultServer == name)
                    {
                        item.IsChecked = true;
                        this.Default = item;
                    }
                }
            }
        }

        public ServerItemModel Default
        {
            get
            {
                return this.@default;
            }
            set
            {
                if (value == this.@default) return;
                this.@default = value;
                foreach (var item in this)
                {
                    item.IsChecked = this.@default == item;
                }
                if (value != null)
                {
                    using (var settings = new ApplicationRegistrySettings())
                    {
                        settings.DefaultServer = this.@default.Name;
                    }
                }
                events.PublishOnBackgroundThread(new ServerChanged(value));
                NotifyOfPropertyChange("Default");
            }
        }

        public static BindableServerCollection Instance
        {
            get
            {
                return instance;
            }
        }

        public string DefaultName
        {
            get
            {
                if (Instance.Default != null)
                {
                    return Instance.Default.Name;
                }

                return null;
            }
        }
    }
}