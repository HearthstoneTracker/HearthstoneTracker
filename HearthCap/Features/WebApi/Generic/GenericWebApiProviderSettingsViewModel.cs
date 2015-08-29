using System;
using System.ComponentModel;
using Caliburn.Micro.Recipes.Filters;

namespace HearthCap.Features.WebApi.Generic
{
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
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsEnabled":
                case "Url":
                case "ApiKey":
                case "SecretKey":
                    ValuesChanged = true;
                    break;
            }
        }

        public virtual string Url
        {
            get { return url; }
            set
            {
                if (value == url)
                {
                    return;
                }
                url = value;
                NotifyOfPropertyChange(() => Url);
            }
        }

        public virtual string ApiKey
        {
            get { return apiKey; }
            set
            {
                if (value == apiKey)
                {
                    return;
                }
                apiKey = value;
                NotifyOfPropertyChange(() => ApiKey);
            }
        }

        public virtual string SecretKey
        {
            get { return secretKey; }
            set
            {
                if (value == secretKey)
                {
                    return;
                }
                secretKey = value;
                NotifyOfPropertyChange(() => SecretKey);
            }
        }

        public bool ValuesChanged
        {
            get { return valuesChanged; }
            set
            {
                if (value.Equals(valuesChanged))
                {
                    return;
                }
                valuesChanged = value;
                NotifyOfPropertyChange(() => ValuesChanged);
            }
        }

        /// <summary>
        ///     Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
        }

        [Dependencies("Url", "ApiKey", "SecretKey", "IsEnabled", "ValuesChanged")]
        public virtual void Save()
        {
            using (var reg = new WebApiProviderSettings(providerDescriptor.ProviderKey))
            {
                reg.Enabled = IsEnabled;
                reg.Url = Url;
                reg.ApiKey = ApiKey;
                reg.SecretKey = SecretKey;
            }
            providerDescriptor.IsEnabled = IsEnabled;
            providerDescriptor.Data["Url"] = Url ?? String.Empty;
            providerDescriptor.Data["ApiKey"] = ApiKey ?? String.Empty;
            providerDescriptor.Data["SecretKey"] = SecretKey ?? String.Empty;
            ((IWebApiProviderDescriptor)providerDescriptor).Initialize();
        }

        public virtual bool CanSave()
        {
            var valid = true;
            Uri uri;
            if (!Uri.TryCreate(Url, UriKind.Absolute, out uri))
            {
                valid = false;
            }
            valid = valid && !String.IsNullOrWhiteSpace(ApiKey);
            valid = valid && !String.IsNullOrWhiteSpace(SecretKey);
            return valid && ValuesChanged;
        }

        protected override void LoadSettings()
        {
            IsNotifying = false;
            using (var reg = new WebApiProviderSettings(providerDescriptor.ProviderKey))
            {
                IsEnabled = reg.Enabled;
                Url = !String.IsNullOrEmpty(reg.Url) ? reg.Url : Url;
                ApiKey = !String.IsNullOrEmpty(reg.ApiKey) ? reg.ApiKey : ApiKey;
                SecretKey = !String.IsNullOrEmpty(reg.SecretKey) ? reg.SecretKey : SecretKey;
            }

            providerDescriptor.IsEnabled = IsEnabled;
            providerDescriptor.Data["Url"] = Url ?? String.Empty;
            providerDescriptor.Data["ApiKey"] = ApiKey ?? String.Empty;
            providerDescriptor.Data["SecretKey"] = SecretKey ?? String.Empty;
            IsNotifying = true;
            Refresh();
        }
    }
}
