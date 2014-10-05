// --------------------------------------------------------------------------------------------------------------------
// <copyright file="AddUserAgentHandler.cs" company="">
//   
// </copyright>
// <summary>
//   The add user agent handler.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.HearthstoneTracker
{
    using System;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Reflection;
    using System.Threading;
    using System.Threading.Tasks;

    /// <summary>
    /// The add user agent handler.
    /// </summary>
    public class AddUserAgentHandler : DelegatingHandler
    {
        /// <summary>
        /// The version.
        /// </summary>
        private static Version version = Assembly.GetEntryAssembly().GetName().Version;

        /// <summary>
        /// The send async.
        /// </summary>
        /// <param name="request">
        /// The request.
        /// </param>
        /// <param name="cancellationToken">
        /// The cancellation token.
        /// </param>
        /// <returns>
        /// The <see cref="Task"/>.
        /// </returns>
        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request.Headers.UserAgent.Clear();
            request.Headers.UserAgent.Add(new ProductInfoHeaderValue("HearthstoneTracker", this.GetVersion()));
            var response = await base.SendAsync(request, cancellationToken);
            return response;
        }

        /// <summary>
        /// The get version.
        /// </summary>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        private string GetVersion()
        {
            return string.Format("{0}.{1}.{2}", version.Major, version.Minor, version.Build);
        }
    }
}