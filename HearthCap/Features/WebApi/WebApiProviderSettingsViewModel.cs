// --------------------------------------------------------------------------------------------------------------------
// <copyright file="WebApiProviderSettingsViewModel.cs" company="">
//   
// </copyright>
// <summary>
//   The web api provider settings view model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi
{
    using Caliburn.Micro;

    /// <summary>
    /// The web api provider settings view model.
    /// </summary>
    public abstract class WebApiProviderSettingsViewModel : Screen, IWebApiProviderScreen
    {
        /// <summary>
        /// The is enabled.
        /// </summary>
        private bool isEnabled;

        /// <summary>
        /// Gets or sets a value indicating whether is enabled.
        /// </summary>
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

        /// <summary>
        /// The initialize.
        /// </summary>
        void IWebApiProviderScreen.Initialize()
        {
            this.LoadSettings();
        }

        /// <summary>
        /// The load settings.
        /// </summary>
        protected abstract void LoadSettings();
    }
}