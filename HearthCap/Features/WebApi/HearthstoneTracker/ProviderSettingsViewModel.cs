// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProviderSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The provider settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    using System.ComponentModel;

    using HearthCap.Features.WebApi.Generic;

    /// <summary>
    /// The provider settings view model.
    /// </summary>
    public class ProviderSettingsViewModel : GenericWebApiProviderSettingsViewModel
    {
        /// <summary>
        /// The hs tracker url.
        /// </summary>
        private const string HSTrackerUrl = "http://hearthstonetracker.com/api/live";

        /// <summary>
        /// The url.
        /// </summary>
        private string url;

        /// <summary>
        /// Initializes a new instance of the <see cref="ProviderSettingsViewModel"/> class.
        /// </summary>
        /// <param name="providerDescriptor">
        /// The provider descriptor.
        /// </param>
        public ProviderSettingsViewModel(WebApiProviderDescriptor providerDescriptor)
            : base(providerDescriptor)
        {
            this.Url = HSTrackerUrl;
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
                case "Url":
                    this.ValuesChanged = true;
                    break;
            }
        }

        /// <summary>
        /// Gets or sets the url.
        /// </summary>
        public override string Url
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
    }
}