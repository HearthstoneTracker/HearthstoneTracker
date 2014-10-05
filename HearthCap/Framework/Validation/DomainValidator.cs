// --------------------------------------------------------------------------------------------------------------------
// <copyright company="" file="DomainValidator.cs">
//   
// </copyright>
// <summary>
//   The domain validator.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace HearthCap.Framework.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// The domain validator.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited = true)]
    public class DomainValidator : RegularExpressionAttribute, IValidationControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DomainValidator"/> class.
        /// </summary>
        public DomainValidator()
            : base(@"^[a-z]+([a-z0-9+-]+)*\\[a-z]+(\.[a-z0-9+-]+)*$") 
        { }

        /// <summary>
        /// The is valid.
        /// </summary>
        /// <param name="value">
        /// The value.
        /// </param>
        /// <returns>
        /// The <see cref="bool"/>.
        /// </returns>
        public override bool IsValid(object value)
        {
            return base.IsValid(value);
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
