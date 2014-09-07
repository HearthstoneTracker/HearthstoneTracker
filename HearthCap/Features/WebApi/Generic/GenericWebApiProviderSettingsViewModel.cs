namespace HearthCap.Features.WebApi.Generic
{
    using System;
    using System.ComponentModel;

    using Caliburn.Micro.Recipes.Filters;

    public class GenericWebApiProviderSettingsViewModel : WebApiProviderSettingsViewModel
    {
        private readonly WebApiProviderDescriptor providerDescriptor;

        private string url;

        private string apiKey;

        private string secretKey;

        private bool valuesChanged;

        public GenericWebApiProviderSettingsViewModel(WebApiProviderDescriptor providerDescriptor)
        {
            if (providerDescriptor == null)
            {
                throw new ArgumentNullException("providerDescriptor");
            }
            Url = String.Empty;
            ApiKey = String.Empty;
            SecretKey = String.Empty;

            this.providerDescriptor = providerDescriptor;
            this.PropertyChanged += OnPropertyChanged;
        }

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
            this.providerDescriptor.Data["Url"] = this.Url ?? String.Empty;
            this.providerDescriptor.Data["ApiKey"] = this.ApiKey ?? String.Empty;
            this.providerDescriptor.Data["SecretKey"] = this.SecretKey ?? String.Empty;
            ((IWebApiProviderDescriptor)this.providerDescriptor).Initialize();
        }

        public virtual bool CanSave()
        {
            bool valid = true;
            Uri uri;
            if (!Uri.TryCreate(this.Url, UriKind.Absolute, out uri))
            {
                valid = false;
            }
            valid = valid && !String.IsNullOrWhiteSpace(this.ApiKey);
            valid = valid && !String.IsNullOrWhiteSpace(this.SecretKey);
            return valid && ValuesChanged;
        }

        protected override void LoadSettings()
        {
            IsNotifying = false;
            using (var reg = new WebApiProviderSettings(this.providerDescriptor.ProviderKey))
            {
                this.IsEnabled = reg.Enabled;
                this.Url = !String.IsNullOrEmpty(reg.Url) ? reg.Url : this.Url;
                this.ApiKey = !String.IsNullOrEmpty(reg.ApiKey) ? reg.ApiKey : this.ApiKey;
                this.SecretKey = !String.IsNullOrEmpty(reg.SecretKey) ? reg.SecretKey : this.SecretKey;
            }

            this.providerDescriptor.IsEnabled = this.IsEnabled;
            this.providerDescriptor.Data["Url"] = this.Url ?? String.Empty;
            this.providerDescriptor.Data["ApiKey"] = this.ApiKey ?? String.Empty;
            this.providerDescriptor.Data["SecretKey"] = this.SecretKey ?? String.Empty;
            IsNotifying = true;
            Refresh();
        }
    }
}