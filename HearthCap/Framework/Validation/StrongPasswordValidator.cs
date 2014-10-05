// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="StrongPasswordValidator.cs">
//   
// </copyright>
// <summary>
//   Validate an entry as a valid domain in the form [domain]\[accountname]
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HearthCap.Framework.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Validate an entry as a valid domain in the form [domain]\[accountname]
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class StrongPasswordValidatorAttribute
        : RegularExpressionAttribute, IValidationControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="StrongPasswordValidatorAttribute"/> class. 
        /// Default constructor
        /// </summary>
        public StrongPasswordValidatorAttribute()
            : base(@"^.*(?=.{8,})(?=.*\d)(?=.*[a-z])(?=.*[A-Z])(?=.*[@#$%^&+=]).*$") { }

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
