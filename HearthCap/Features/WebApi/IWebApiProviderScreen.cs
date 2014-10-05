// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebApiProviderScreen.cs" company="">
//   
// </copyright>
// <summary>
//   The WebApiProviderScreen interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi
{
    using Caliburn.Micro;

    /// <summary>
    /// The WebApiProviderScreen interface.
    /// </summary>
    public interface IWebApiProviderScreen : IScreen
    {
        /// <summary>
        /// Gets or sets a value indicating whether is enabled.
        /// </summary>
        bool IsEnabled { get; set; }

        /// <summary>
        /// The initialize.
        /// </summary>
        void Initialize();
    }
}