// --------------------------------------------------------------------------------------------------------------------
// <copyright file="HmacSignatureCalculator.cs" company="">
//   
// </copyright>
// <summary>
//   The hmac signature calculator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.WebApi.Hmac
{
    using System;
    using System.Security.Cryptography;
    using System.Text;

    /// <summary>
    /// The hmac signature calculator.
    /// </summary>
    public class HmacSignatureCalculator : ICalculteSignature
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
        public string Signature(string secret, string value)
        {
            var secretBytes = Encoding.UTF8.GetBytes(secret);
            var valueBytes = Encoding.UTF8.GetBytes(value);
            string signature;

            using (var hmac = new HMACSHA256(secretBytes))
            {
                var hash = hmac.ComputeHash(valueBytes);
                signature = Convert.ToBase64String(hash);
            }

            return signature;
        }
    }
}