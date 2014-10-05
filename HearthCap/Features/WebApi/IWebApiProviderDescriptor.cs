// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebApiProviderDescriptor.cs" company="">
//   
// </copyright>
// <summary>
//   The WebApiProviderDescriptor interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi
{
    using System.ComponentModel;

    /// <summary>
    /// The WebApiProviderDescriptor interface.
    /// </summary>
    public interface IWebApiProviderDescriptor : INotifyPropertyChanged
    {
        /// <summary>
        /// Gets the provider key.
        /// </summary>
        string ProviderKey { get; }

        /// <summary>
        /// Gets or sets the provider name.
        /// </summary>
        string ProviderName { get; set; }

        /// <summary>
        /// Gets or sets the provider description.
        /// </summary>
        string ProviderDescription { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether is enabled.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// Gets the events handler.
        /// </summary>
        IWebApiEventsHandler EventsHandler { get; }

        /// <summary>
        /// The initialize.
        /// </summary>
        void Initialize();
    }
}