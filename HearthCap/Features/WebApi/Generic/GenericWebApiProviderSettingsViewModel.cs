// --------------------------------------------------------------------------------------------------------------------
// <copyright file="GenericWebApiProviderSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The generic web api provider settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Generic
{
    using System;
    using System.ComponentModel;

    using Caliburn.Micro.Recipes.Filters;

    /// <summary>
    /// The generic web api provider settings view model.
    /// </summary>
    public class GenericWebApiProviderSettingsViewModel : WebApiProviderSettingsViewModel
    {
        /// <summary>
        /// The provider descriptor.
        /// </summary>
        private readonly WebApiProviderDescriptor providerDescriptor;

        /// <summary>
        /// The url.
        /// </summary>
        private string url;

        /// <summary>
        /// The api key.
        /// </summary>
        private string apiKey;

        /// <summary>
        /// The secret key.
        /// </summary>
        private string secretKey;

        /// <summary>
        /// The values changed.
        /// </summary>
        private bool valuesChanged;

        /// <summary>
        /// Initializes a new instance of the <see cref="GenericWebApiProviderSettingsViewModel"/> class.
        /// </summary>
        /// <param name="providerDescriptor">
        /// The provider descriptor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public GenericWebApiProviderSettingsViewModel(WebApiProviderDescriptor providerDescriptor)
        {
            if (providerDescriptor == null)
            {
                throw new ArgumentNullException("providerDescriptor");
            }

            this.Url = string.Empty;
            this.ApiKey = string.Empty;
            this.SecretKey = string.Empty;

            this.providerDescriptor = providerDescriptor;
            this.PropertyChanged += this.OnPropertyChanged;
        }

        /// <summary>
        /// The on property changed.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsEnabled":
                case "Url":
                case "ApiKey":
                case "SecretKey":
                    this.ValuesChanged = true;
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public virtual string Url
        {
            get
            {
                return this.url;
            }

            set
            {
                if (value == this.url)
                {
                    return;
                }

                this.url = value;
                this.NotifyOfPropertyChange(() => this.Url);
            }
        }

        /// <summary>
        /// Gets or sets the api key.
        /// </summary>
        public virtual string ApiKey
        {
            get
            {
                return this.apiKey;
            }

            set
            {
                if (value == this.apiKey)
                {
                    return;
                }

                this.apiKey = value;
                this.NotifyOfPropertyChange(() => this.ApiKey);
            }
        }

        /// <summary>
        /// Gets or sets the secret key.
        /// </summary>
        public virtual string SecretKey
        {
            get
            {
                return this.secretKey;
            }

            set
            {
                if (value == this.secretKey)
                {
                    return;
                }

                this.secretKey = value;
                this.NotifyOfPropertyChange(() => this.SecretKey);
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether values changed.
        /// </summary>
        public bool ValuesChanged
        {
            get
            {
                return this.valuesChanged;
            }

            set
            {
                if (value.Equals(this.valuesChanged)) return;
                this.valuesChanged = value;
                this.NotifyOfPropertyChange(() => this.ValuesChanged);
            }
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
        }

        /// <summary>
        /// The save.
        /// </summary>
        [Dependencies("Url", "ApiKey", "SecretKey", "IsEnabled", "ValuesChanged")]
        public virtual void Save()
        {
            using (var reg = new WebApiProviderSettings(this.providerDescriptor.ProviderKey))
            {
                reg.Enabled = this.IsEnabled;
                reg.Url = this.Url;
                reg.ApiKey = this.ApiKey;
                reg.SecretKey = this.SecretKey;
            }

            this.providerDescriptor.IsEnabled = this.IsEnabled;
            this.providerDescriptor.Data["Url"] = this.Url ?? string.Empty;
            this.providerDescriptor.Data["ApiKey"] = this.ApiKey ?? string.Empty;
            this.providerDescriptor.Data["SecretKey"] = this.SecretKey ?? string.Empty;
            ((IWebApiProviderDescriptor)this.providerDescriptor).Initialize();
        }

        /// <summary>
        /// The can save.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public virtual bool CanSave()
        {
            bool valid = true;
            Uri uri;
            if (!Uri.TryCreate(this.Url, UriKind.Absolute, out uri))
            {
                valid = false;
            }

            valid = valid && !string.IsNullOrWhiteSpace(this.ApiKey);
            valid = valid && !string.IsNullOrWhiteSpace(this.SecretKey);
            return valid && this.ValuesChanged;
        }

        /// <summary>
        /// The load settings.
        /// </summary>
        protected override void LoadSettings()
        {
            this.IsNotifying = false;
            using (var reg = new WebApiProviderSettings(this.providerDescriptor.ProviderKey))
            {
                this.IsEnabled = reg.Enabled;
                this.Url = !string.IsNullOrEmpty(reg.Url) ? reg.Url : this.Url;
                this.ApiKey = !string.IsNullOrEmpty(reg.ApiKey) ? reg.ApiKey : this.ApiKey;
                this.SecretKey = !string.IsNullOrEmpty(reg.SecretKey) ? reg.SecretKey : this.SecretKey;
            }

            this.providerDescriptor.IsEnabled = this.IsEnabled;
            this.providerDescriptor.Data["Url"] = this.Url ?? string.Empty;
            this.providerDescriptor.Data["ApiKey"] = this.ApiKey ?? string.Empty;
            this.providerDescriptor.Data["SecretKey"] = this.SecretKey ?? string.Empty;
            this.IsNotifying = true;
            this.Refresh();
        }
    }
}