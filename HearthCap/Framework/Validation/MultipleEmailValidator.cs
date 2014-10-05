// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="MultipleEmailValidator.cs">
//   
// </copyright>
// <summary>
//   Supports validating a field containing one or more email
//   addresses where each address is separated by a comma
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HearthCap.Framework.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// Supports validating a field containing one or more email 
    /// addresses where each address is separated by a comma
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class MultipleEmailValidator : RegularExpressionAttribute, IValidationControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MultipleEmailValidator"/> class. 
        /// Default constructor
        /// </summary>
        public MultipleEmailValidator()
            : base(string.Format( @"{0}([,;]\s*{0})*", EmailValidator.EmailValidationExpression)) 
        { }

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
