// --------------------------------------------------------------------------------------------------------------------
// <copyright file="MessageRepresentation.cs" company="">
//   
// </copyright>
// <summary>
//   The message representation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Hmac
{
    using System;

    /// <summary>
    /// The message representation.
    /// </summary>
    public class MessageRepresentation
    {
        /// <summary>
        /// Gets or sets the representation.
        /// </summary>
        public string Representation { get; set; }

        /// <summary>
        /// Gets or sets the api key.
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// Gets or sets the date.
        /// </summary>
        public DateTime Date { get; set; }
    }
}