// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IWebApiEventsHandler.cs" company="">
//   
// </copyright>
// <summary>
//   The WebApiEventsHandler interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi
{
    /// <summary>
    /// The WebApiEventsHandler interface.
    /// </summary>
    public interface IWebApiEventsHandler
    {
        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="providerDescriptor">
        /// The provider descriptor.
        /// </param>
        void Initialize(WebApiProviderDescriptor providerDescriptor);
    }
}