using System;
using System.ComponentModel;
using Caliburn.Micro.Recipes.Filters;

namespace HearthCap.Features.WebApi.Twitter
{
    public class TwitterSettingsViewModel : WebApiProviderSettingsViewModel
    {
        private readonly WebApiProviderDescriptor providerDescriptor;

        private bool valuesChanged;

        private string pinCode;

        public TwitterSettingsViewModel(WebApiProviderDescriptor providerDescriptor)
        {
            if (providerDescriptor == null)
            {
                throw new ArgumentNullException("providerDescriptor");
            }
            this.providerDescriptor = providerDescriptor;
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsEnabled":
                case "PinCode":
                    ValuesChanged = true;
                    break;
            }
        }

        public string PinCode
        {
            get { return pinCode; }
            set
            {
                if (value == pinCode)
                {
                    return;
                }
                pinCode = value;
                NotifyOfPropertyChange(() => PinCode);
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

        [Dependencies("PinCode", "IsEnabled")]
        public void Save()
        {
            using (var reg = new ProviderSettings(providerDescriptor.ProviderKey))
            {
                reg.Enabled = IsEnabled;
                reg.SetValue("PinCode", PinCode);
            }
            providerDescriptor.IsEnabled = IsEnabled;
            providerDescriptor.Data["PinCode"] = PinCode;
            ((IWebApiProviderDescriptor)providerDescriptor).Initialize();
        }

        public bool CanSave()
        {
            var valid = true;
            valid = valid && !String.IsNullOrWhiteSpace(PinCode);
            return valid && ValuesChanged;
        }

        protected override void LoadSettings()
        {
            IsNotifying = false;
            using (var reg = new ProviderSettings(providerDescriptor.ProviderKey))
            {
                IsEnabled = reg.Enabled;
                PinCode = reg.GetOrCreate("PinCode", "");
            }
            providerDescriptor.IsEnabled = IsEnabled;
            providerDescriptor.Data["PinCode"] = PinCode;
            IsNotifying = true;
            Refresh();
        }
    }
}
