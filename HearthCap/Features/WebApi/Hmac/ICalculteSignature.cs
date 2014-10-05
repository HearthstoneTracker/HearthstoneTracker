// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ICalculteSignature.cs" company="">
//   
// </copyright>
// <summary>
//   The CalculteSignature interface.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Hmac
{
    /// <summary>
    /// The CalculteSignature interface.
    /// </summary>
    public interface ICalculteSignature
    {
        /// <summary>
        /// The signature.
        /// </summary>
        /// <param name="secret">
        /// The secret.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="string"/>.
        /// </returns>
        string Signature(string secret, string value);
    }
}