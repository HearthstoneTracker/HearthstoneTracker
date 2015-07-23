namespace HearthCap.Features.WebApi.Twitter
{
    using System;
    using System.ComponentModel;

    using Caliburn.Micro.Recipes.Filters;

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
            this.PropertyChanged += this.OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsEnabled":
                case "PinCode":
                    this.ValuesChanged = true;
                    break;
            }
        }

        public string PinCode
        {
            get
            {
                return this.pinCode;
            }
            set
            {
                if (value == this.pinCode)
                {
                    return;
                }
                this.pinCode = value;
                this.NotifyOfPropertyChange(() => this.PinCode);
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

        [Dependencies("PinCode", "IsEnabled")]
        public void Save()
        {
            using (var reg = new ProviderSettings(this.providerDescriptor.ProviderKey))
            {
                reg.Enabled = this.IsEnabled;
                reg.SetValue("PinCode", this.PinCode);
            }
            this.providerDescriptor.IsEnabled = this.IsEnabled;
            this.providerDescriptor.Data["PinCode"] = this.PinCode;
            ((IWebApiProviderDescriptor)this.providerDescriptor).Initialize();
        }

        public bool CanSave()
        {
            bool valid = true;
            valid = valid && !String.IsNullOrWhiteSpace(this.PinCode);
            return valid && this.ValuesChanged;
        }

        protected override void LoadSettings()
        {
            this.IsNotifying = false;
            using (var reg = new ProviderSettings(this.providerDescriptor.ProviderKey))
            {
                this.IsEnabled = reg.Enabled;
                this.PinCode = reg.GetOrCreate("PinCode", "");
            }
            this.providerDescriptor.IsEnabled = this.IsEnabled;
            this.providerDescriptor.Data["PinCode"] = this.PinCode;
            this.IsNotifying = true;
            this.Refresh();
        }

    }
}