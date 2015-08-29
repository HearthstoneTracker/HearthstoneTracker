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
using System.IO;

namespace HearthCap.Framework.Validation
{
    /// <summary>
    ///     Validates an entered path exists
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class PathValidator : ValidationAttribute, IValidationControl
    {
        /// <summary>
        ///     If the value is a string, the string is checked that its a valid path.
        ///     If a relative path, that the path exist relative to the current directory.
        /// </summary>
        /// <param name="value">The path to be validated</param>
        /// <returns>True if the path is found</returns>
        public override bool IsValid(object value)
        {
            // The value should be a string representing a valid path
            if (value is string)
            {
                return Directory.Exists((string)value);
            }
            return false;
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
