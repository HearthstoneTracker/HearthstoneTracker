// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ValidationGroupAttribute.cs" company="">
//   
// </copyright>
// <summary>
//   Allows a property's validation by validating controller class to be broken into
//   groups.  If a validation group is not defined then a validating controller class
//   is able to assume validation attributes instances are in a 'default' group and
//   that the validation will be applied during Error evaluation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Framework.Validation
{
    using System;

    /// <summary>
    /// Allows a property's validation by validating controller class to be broken into 
    /// groups.  If a validation group is not defined then a validating controller class 
    /// is able to assume validation attributes instances are in a 'default' group and
    /// that the validation will be applied during Error evaluation.
    /// </summary>
    /// <remarks>
    /// This attribute is intended to be used by a 'validating controller' class which 
    /// implements IDataErrorInfo.  The ValidatingScreen is an example of a controller
    /// </remarks>
    public class ValidationGroupAttribute : Attribute
    {
        /// <summary>
        /// The default group name.
        /// </summary>
        public const string DefaultGroupName = "Default";

        /// <summary>
        /// Initializes a new instance of the <see cref="ValidationGroupAttribute"/> class. 
        /// Default constructor
        /// </summary>
        public ValidationGroupAttribute()
            : base()
        {
            this.GroupName = DefaultGroupName;
            this.IncludeInErrorsValidation = true;
        }

        /// <summary>
        /// A name which defines a group of properties which should be validated together 
        /// </summary>
        public string GroupName { get; set; }

        /// <summary>
        /// Get/Set as flag to indicate whether validation for a field should be applied when Error/HasError is applied
        /// </summary>
        public bool IncludeInErrorsValidation { get; set; }
    }
}
