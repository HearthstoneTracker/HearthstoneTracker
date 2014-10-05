// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IBuildMessageRepresentation.cs" company="">
//   
// </copyright>
// <summary>
//   The BuildMessageRepresentation interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Hmac
{
    using System.Net.Http;

    /// <summary>
    /// The BuildMessageRepresentation interface.
    /// </summary>
    public interface IBuildMessageRepresentation
    {
        /// <summary>
        /// The build request representation.
        /// </summary>
        /// <param name="requestMessage">
        /// The request message.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string BuildRequestRepresentation(HttpRequestMessage requestMessage);
    }
}