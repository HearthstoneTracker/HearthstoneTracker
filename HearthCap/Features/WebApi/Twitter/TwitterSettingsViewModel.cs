// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TwitterSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The twitter settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HearthCap.Features.WebApi.Twitter
{
    using System;
    using System.ComponentModel;

    using Caliburn.Micro.Recipes.Filters;

    /// <summary>
    /// The twitter settings view model.
    /// </summary>
    public class TwitterSettingsViewModel : WebApiProviderSettingsViewModel
    {
        /// <summary>
        /// The provider descriptor.
        /// </summary>
        private readonly WebApiProviderDescriptor providerDescriptor;

        /// <summary>
        /// The values changed.
        /// </summary>
        private bool valuesChanged;

        /// <summary>
        /// The pin code.
        /// </summary>
        private string pinCode;

        /// <summary>
        /// Initializes a new instance of the <see cref="TwitterSettingsViewModel"/> class.
        /// </summary>
        /// <param name="providerDescriptor">
        /// The provider descriptor.
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// </exception>
        public TwitterSettingsViewModel(WebApiProviderDescriptor providerDescriptor)
        {
            if (providerDescriptor == null)
            {
                throw new ArgumentNullException("providerDescriptor");
            }

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
                case "PinCode":
                    this.ValuesChanged = true;
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the pin code.
        /// </summary>
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

        /// <summary>
        /// The can save.
        /// </summary>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public bool CanSave()
        {
            bool valid = !string.IsNullOrWhiteSpace(this.PinCode);
            return valid && this.ValuesChanged;
        }

        /// <summary>
        /// The load settings.
        /// </summary>
        protected override void LoadSettings()
        {
            this.IsNotifying = false;
            using (var reg = new ProviderSettings(this.providerDescriptor.ProviderKey))
            {
                this.IsEnabled = reg.Enabled;
                this.PinCode = reg.GetOrCreate("PinCode", string.Empty);
            }

            this.providerDescriptor.IsEnabled = this.IsEnabled;
            this.providerDescriptor.Data["PinCode"] = this.PinCode;
            this.IsNotifying = true;
            this.Refresh();
        }

    }
}