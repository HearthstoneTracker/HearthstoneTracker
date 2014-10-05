// --------------------------------------------------------------------------------------------------------------------
// <copyright file="TwitterApiEventsHandler.cs" company="">
//   
// </copyright>
// <summary>
//   The twitter api events handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Twitter
{
    /// <summary>
    /// The twitter api events handler.
    /// </summary>
    public class TwitterApiEventsHandler : IWebApiEventsHandler
    {
        /// <summary>
        /// The provider descriptor.
        /// </summary>
        private WebApiProviderDescriptor providerDescriptor;

        /// <summary>
        /// The initialize.
        /// </summary>
        /// <param name="providerDescriptor">
        /// The provider descriptor.
        /// </param>
        public void Initialize(WebApiProviderDescriptor providerDescriptor)
        {
            this.providerDescriptor = providerDescriptor;
        }
    }
}