using System.ComponentModel;
using HearthCap.Features.WebApi.Generic;

namespace HearthCap.Features.WebApi.ValueBot
{
    public class ProviderSettingsViewModel : GenericWebApiProviderSettingsViewModel
    {
        public ProviderSettingsViewModel(WebApiProviderDescriptor providerDescriptor)
            : base(providerDescriptor)
        {
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
        }

        /// <summary>
        ///     Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }
    }
}
