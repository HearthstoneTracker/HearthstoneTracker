// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SettingModel.cs" company="">
//   
// </copyright>
// <summary>
//   The setting model.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Features.Settings
{
    /// <summary>
    /// The setting model.
    /// </summary>
    public class SettingModel
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingModel"/> class.
        /// </summary>
        public SettingModel()
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingModel"/> class.
        /// </summary>
        /// <param name="name">
        /// The name.
        /// </param>
        /// <param name="value">
        /// The value.
        /// </param>
        public SettingModel(string name, object value)
        {
            this.Name = name;
            this.Value = value;
        }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets the value.
        /// </summary>
        public object Value { get; set; }
    }
}