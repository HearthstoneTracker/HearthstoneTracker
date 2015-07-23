namespace HearthCap.Features.WebApi.ValueBot
{
    using System;
    using System.ComponentModel;

    using HearthCap.Features.WebApi.Generic;

    public class ProviderSettingsViewModel : GenericWebApiProviderSettingsViewModel
    {
        public ProviderSettingsViewModel(WebApiProviderDescriptor providerDescriptor)
            : base(providerDescriptor)
        {
            this.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }
    }
}