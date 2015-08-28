namespace HearthCap.Framework.Validation
{
    /// <summary>
	/// Interface allows a validation controller to refine its validation
	/// and, for example, restrict validation to enabled properties
	/// </summary>
	public interface IValidationControl
	{
		/// <summary>
		/// When true a validation controller will 
		/// </summary>
		bool ValidateWhileDisabled { get; set; }

		/// <summary>
		/// If not defined the guard property to check for disabled state is Can[PropertyName]
		/// However it may be necessary to test another guard property and this is the place 
		/// to specify the alternative property to query.
		/// </summary>
		string GuardProperty { get; set; }
	}
}
