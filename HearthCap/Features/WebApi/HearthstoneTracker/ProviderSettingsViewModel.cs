namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    using System;
    using System.ComponentModel;

    using HearthCap.Features.WebApi.Generic;

    public class ProviderSettingsViewModel : GenericWebApiProviderSettingsViewModel
    {
        private const string HSTrackerUrl = "http://hearthstonetracker.com/api/live";

        private string url;

        public ProviderSettingsViewModel(WebApiProviderDescriptor providerDescriptor)
            : base(providerDescriptor)
        {
            Url = HSTrackerUrl;
            this.PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Url":
                    this.ValuesChanged = true;
                    break;
            }
        }

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

        /// <summary>
        /// Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }
    }
}