using System;
using Caliburn.Micro;
using HearthCap.Shell.UserPreferences;

namespace HearthCap.Features.Core
{
    public class BindableServerCollection : BindableCollection<ServerItemModel>
    {
        private readonly IEventAggregator events;

        private static readonly BindableServerCollection instance = new BindableServerCollection();

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
                        Default = item;
                    }
                }
            }
        }

        public ServerItemModel Default
        {
            get { return @default; }
            set
            {
                if (value == @default)
                {
                    return;
                }
                @default = value;
                foreach (var item in this)
                {
                    item.IsChecked = @default == item;
                }
                if (value != null)
                {
                    using (var settings = new ApplicationRegistrySettings())
                    {
                        settings.DefaultServer = @default.Name;
                    }
                }
                events.PublishOnBackgroundThread(new ServerChanged(value));
                NotifyOfPropertyChange("Default");
            }
        }

        public static BindableServerCollection Instance
        {
            get { return instance; }
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
