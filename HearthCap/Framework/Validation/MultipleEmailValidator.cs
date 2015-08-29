/******************************************************************************* 
 *  _                      _     _ _ _         
 * | |   _   _  __ _ _   _(_) __| (_) |_ _   _ 
 * | |  | | | |/ _` | | | | |/ _` | | __| | | |
 * | |__| |_| | (_| | |_| | | (_| | | |_| |_| |
 * |_____\__, |\__, |\__,_|_|\__,_|_|\__|\__, |
 *       |___/    |_|                    |___/ 
 * 
 *  Lyquidity AmazonSES for Exchange
 *  Version: 1.0.0.1
 *  Generated: Monday Jan 31 20:00:00 GMT 2011 
 *  *
 * ***************************************************************************** 
 *  Copyright Lyquidity Solutions Limited 2011
 * ***************************************************************************** 
 * 
 */

using System;
using System.ComponentModel.DataAnnotations;

namespace HearthCap.Framework.Validation
{
    /// <summary>
    ///     Supports validating a field containing one or more email
    ///     addresses where each address is separated by a comma
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class MultipleEmailValidator : RegularExpressionAttribute, IValidationControl
    {
        /// <summary>
        ///     Default constructor
        /// </summary>
        public MultipleEmailValidator()
            : base(string.Format(@"{0}([,;]\s*{0})*", EmailValidator.EmailValidationExpression))
        {
        }

        #region IValidationControl

        /// <summary>
        ///     When true a validation controller will
        /// </summary>
        public bool ValidateWhileDisabled { get; set; }

        /// <summary>
        ///     If not defined the guard property to check for disabled state is Can[PropertyName]
        ///     However it may be necessary to test another guard property and this is the place
        ///     to specify the alternative property to query.
        /// </summary>
        public string GuardProperty { get; set; }

        #endregion
    }
}
