namespace HearthCap.Features.WebApi
{
    using System.Collections;
    using System.Collections.Generic;

    using Caliburn.Micro;

    public abstract class WebApiProviderDescriptor : PropertyChangedBase, IWebApiProviderDescriptor
    {
        private string providerName;

        private string providerDescription;

        private readonly string providerKey;

        private bool isEnabled;

        protected WebApiProviderDescriptor(string providerKey)
        {
            this.providerKey = providerKey;
            this.Data = new Dictionary<string, string>();
        }

        public string ProviderKey
        {
            get
            {
                return providerKey;
            }
        }

        public string ProviderName
        {
            get
            {
                return this.providerName;
            }
            set
            {
                if (value == this.providerName)
                {
                    return;
                }
                this.providerName = value;
                this.NotifyOfPropertyChange(() => this.ProviderName);
            }
        }

        public string ProviderDescription
        {
            get
            {
                return this.providerDescription;
            }
            set
            {
                if (value == this.providerDescription)
                {
                    return;
                }
                this.providerDescription = value;
                this.NotifyOfPropertyChange(() => this.ProviderDescription);
            }
        }

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                if (value.Equals(this.isEnabled))
                {
                    return;
                }
                this.isEnabled = value;
                this.NotifyOfPropertyChange(() => this.IsEnabled);
            }
        }

        public abstract IWebApiProviderScreen Settings { get; }

        public abstract IWebApiEventsHandler EventsHandler { get; }

        void IWebApiProviderDescriptor.Initialize()
        {
            this.Settings.Initialize();
            this.EventsHandler.Initialize(this);
        }

        public IDictionary<string, string> Data { get; private set; }
    }
}