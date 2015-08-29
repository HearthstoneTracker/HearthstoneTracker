using System.ComponentModel;
using HearthCap.Features.WebApi.Generic;

namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    public class ProviderSettingsViewModel : GenericWebApiProviderSettingsViewModel
    {
        private const string HSTrackerUrl = "http://hearthstonetracker.com/api/live";

        private string url;

        public ProviderSettingsViewModel(WebApiProviderDescriptor providerDescriptor)
            : base(providerDescriptor)
        {
            Url = HSTrackerUrl;
            PropertyChanged += OnPropertyChanged;
        }

        private void OnPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Url":
                    ValuesChanged = true;
                    break;
            }
        }

        public override string Url
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

        /// <summary>
        ///     Called when initializing.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
        }
    }
}
