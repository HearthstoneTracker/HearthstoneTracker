using System.Collections.Generic;
using Caliburn.Micro;

namespace HearthCap.Features.WebApi
{
    public abstract class WebApiProviderDescriptor : PropertyChangedBase, IWebApiProviderDescriptor
    {
        private string providerName;

        private string providerDescription;

        private readonly string providerKey;

        private bool isEnabled;

        protected WebApiProviderDescriptor(string providerKey)
        {
            this.providerKey = providerKey;
            Data = new Dictionary<string, string>();
        }

        public string ProviderKey
        {
            get { return providerKey; }
        }

        public string ProviderName
        {
            get { return providerName; }
            set
            {
                if (value == providerName)
                {
                    return;
                }
                providerName = value;
                NotifyOfPropertyChange(() => ProviderName);
            }
        }

        public string ProviderDescription
        {
            get { return providerDescription; }
            set
            {
                if (value == providerDescription)
                {
                    return;
                }
                providerDescription = value;
                NotifyOfPropertyChange(() => ProviderDescription);
            }
        }

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (value.Equals(isEnabled))
                {
                    return;
                }
                isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        public abstract IWebApiProviderScreen Settings { get; }

        public abstract IWebApiEventsHandler EventsHandler { get; }

        void IWebApiProviderDescriptor.Initialize()
        {
            Initialize();
        }

        protected void Initialize()
        {
            Settings.Initialize();
            EventsHandler.Initialize(this);
        }

        public IDictionary<string, string> Data { get; private set; }
    }
}
