// --------------------------------------------------------------------------------------------------------------------
// <copyright file="IncompatibleHooksFound.cs" company="">
//   
// </copyright>
// <summary>
//   The incompatible hooks found.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Core.GameCapture.EngineEvents
{
    /// <summary>
    /// The incompatible hooks found.
    /// </summary>
    public class IncompatibleHooksFound : EngineEvent
    {
        /// <summary>
        /// Gets or sets the hook name.
        /// </summary>
        public string HookName { get; set; }

        /// <summary>
        /// Gets or sets the description.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="IncompatibleHooksFound"/> class. 
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        /// <param name="hookName">
        /// The hook Name.
        /// </param>
        /// <param name="description">
        /// The description.
        /// </param>
        public IncompatibleHooksFound(string hookName, string description)
            : base("Incompatible hook found: " + hookName)
        {
            this.HookName = hookName;
            this.Description = description;
        }
    }
}