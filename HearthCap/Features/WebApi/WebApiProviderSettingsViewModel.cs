namespace HearthCap.Features.WebApi
{
    using Caliburn.Micro;

    public abstract class WebApiProviderSettingsViewModel : Screen, IWebApiProviderScreen
    {
        private bool isEnabled;

        public bool IsEnabled
        {
            get
            {
                return this.isEnabled;
            }
            set
            {
                if (value.Equals(this.isEnabled))
                {
                    return;
                }
                this.isEnabled = value;
                this.NotifyOfPropertyChange(() => this.IsEnabled);
            }
        }

        void IWebApiProviderScreen.Initialize()
        {
            this.LoadSettings();
        }

        protected abstract void LoadSettings();
    }
}