namespace HearthCap.Core.GameCapture.EngineEvents
{
    public class IncompatibleHooksFound : EngineEvent
    {
        public string HookName { get; set; }

        public string Description { get; set; }

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public IncompatibleHooksFound(string hookName, string description)
            : base("Incompatible hook found: " + hookName)
        {
            this.HookName = hookName;
            this.Description = description;
        }
    }
}