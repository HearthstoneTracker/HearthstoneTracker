// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiProviderDescriptor.cs" company="">
//   
// </copyright>
// <summary>
//   The web api provider descriptor.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi
{
    using System.Collections.Generic;

    using Caliburn.Micro;

    /// <summary>
    /// The web api provider descriptor.
    /// </summary>
    public abstract class WebApiProviderDescriptor : PropertyChangedBase, IWebApiProviderDescriptor
    {
        /// <summary>
        /// The provider name.
        /// </summary>
        private string providerName;

        /// <summary>
        /// The provider description.
        /// </summary>
        private string providerDescription;

        /// <summary>
        /// The provider key.
        /// </summary>
        private readonly string providerKey;

        /// <summary>
        /// The is enabled.
        /// </summary>
        private bool isEnabled;

        /// <summary>
        /// Initializes a new instance of the <see cref="WebApiProviderDescriptor"/> class.
        /// </summary>
        /// <param name="providerKey">
        /// The provider key.
        /// </param>
        protected WebApiProviderDescriptor(string providerKey)
        {
            this.providerKey = providerKey;
            this.Data = new Dictionary<string, string>();
        }

        /// <summary>
        /// Gets the provider key.
        /// </summary>
        public string ProviderKey
        {
            get
            {
                return this.providerKey;
            }
        }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
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

        /// <summary>
        /// Gets or sets the provider description.
        /// </summary>
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

        /// <summary>
        /// Gets or sets a value indicating whether is enabled.
        /// </summary>
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

        /// <summary>
        /// Gets the settings.
        /// </summary>
        public abstract IWebApiProviderScreen Settings { get; }

        /// <summary>
        /// Gets the events handler.
        /// </summary>
        public abstract IWebApiEventsHandler EventsHandler { get; }

        /// <summary>
        /// The initialize.
        /// </summary>
        void IWebApiProviderDescriptor.Initialize()
        {
            this.Settings.Initialize();
            this.EventsHandler.Initialize(this);
        }

        /// <summary>
        /// Gets the data.
        /// </summary>
        public IDictionary<string, string> Data { get; private set; }
    }
}