namespace HearthCap.Framework.Validation
{
    using System;
    using System.ComponentModel.DataAnnotations;

    /// <summary>
	/// A Required validation attribute sub-classed to allow a validation controller constrain when and how testing is done
	/// </summary>
	[AttributeUsage(AttributeTargets.Property, AllowMultiple = false, Inherited=true)]
	public class RequiredExAttribute : RequiredAttribute, IValidationControl
	{
		#region IValidationControl

		/// <summary>
		/// When true a validation controller will alway validate
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
