// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProviderSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The provider settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.ValueBot
{
    using System.ComponentModel;

    using HearthCap.Features.WebApi.Generic;

    /// <summary>
    /// The provider settings view model.
    /// </summary>
    public class ProviderSettingsViewModel : GenericWebApiProviderSettingsViewModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderSettingsViewModel"/> class.
        /// </summary>
        /// <param name="providerDescriptor">
        /// The provider descriptor.
        /// </param>
        public ProviderSettingsViewModel(WebApiProviderDescriptor providerDescriptor)
            : base(providerDescriptor)
        {
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
        }
    }
}