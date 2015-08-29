using Caliburn.Micro;

namespace HearthCap.Features.WebApi
{
    public abstract class WebApiProviderSettingsViewModel : Screen, IWebApiProviderScreen
    {
        private bool isEnabled;

        public bool IsEnabled
        {
            get { return isEnabled; }
            set
            {
                if (value.Equals(isEnabled))
                {
                    return;
                }
                isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);
            }
        }

        void IWebApiProviderScreen.Initialize()
        {
            Initialize();
        }

        protected void Initialize()
        {
            LoadSettings();
        }

        protected abstract void LoadSettings();
    }
}
