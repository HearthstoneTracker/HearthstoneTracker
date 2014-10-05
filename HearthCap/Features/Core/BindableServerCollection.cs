// --------------------------------------------------------------------------------------------------------------------
// <copyright file="BindableServerCollection.cs" company="">
//   
// </copyright>
// <summary>
//   The bindable server collection.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Core
{
    using System;

    using Caliburn.Micro;

    using HearthCap.Shell.UserPreferences;

    /// <summary>
    /// The bindable server collection.
    /// </summary>
    public class BindableServerCollection : BindableCollection<ServerItemModel>
    {
        /// <summary>
        /// The events.
        /// </summary>
        private IEventAggregator events;

        /// <summary>
        /// The instance.
        /// </summary>
        private readonly static BindableServerCollection instance = new BindableServerCollection();

        /// <summary>
        /// The default.
        /// </summary>
        private ServerItemModel @default;

        /// <summary>
        /// Prevents a default instance of the <see cref="BindableServerCollection"/> class from being created.
        /// </summary>
        private BindableServerCollection()
        {
            this.events = IoC.Get<IEventAggregator>();

            using (var settings = new ApplicationRegistrySettings())
            {
                var serverlist = settings.Servers.Split(new[] { '|' }, StringSplitOptions.RemoveEmptyEntries);
                foreach (var name in serverlist)
                {
                    var item = new ServerItemModel(name);
                    this.Add(item);
                    if (settings.DefaultServer == name)
                    {
                        item.IsChecked = true;
                        this.Default = item;
                    }
                }
            }
        }

        /// <summary>
        /// Gets or sets the default.
        /// </summary>
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

                this.events.PublishOnBackgroundThread(new ServerChanged(value));
                this.NotifyOfPropertyChange("Default");
            }
        }

        /// <summary>
        /// Gets the instance.
        /// </summary>
        public static BindableServerCollection Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// Gets the default name.
        /// </summary>
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