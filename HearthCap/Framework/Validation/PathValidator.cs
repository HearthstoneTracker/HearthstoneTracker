﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="PathValidator.cs">
//   
// </copyright>
// <summary>
//   Validates an entered path exists
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HearthCap.Framework.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;
    using System.IO;

    /// <summary>
    /// Validates an entered path exists
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class PathValidator : ValidationAttribute, IValidationControl
    {
        /// <summary>
        /// If the value is a string, the string is checked that its a valid path.
        /// If a relative path, that the path exist relative to the current directory.
        /// </summary>
        /// <param name="value">
        /// The path to be validated
        /// </param>
        /// <returns>
        /// True if the path is found
        /// </returns>
        public override bool IsValid(object value)
        {
            // The value should be a string representing a valid path
            if (value is string)
                return Directory.Exists((string)value); else
                return false;
        }

        #region IValidationControl

        /// <summary>
        /// When true a validation controller will 
        /// </summary>
        public bool ValidateWhileDisabled { get; set; }

        /// <summary>
        /// If not defined the guard property to check for disabled state is Can[PropertyName]
        /// However it may be necessary to test another guard property and this is the place 
        /// to specify the alternative property to query.
        /// </summary>
        public string GuardProperty { get; set; }

        #endregion
    }
}
