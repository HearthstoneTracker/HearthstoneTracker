// --------------------------------------------------------------------------------------------------------------------
// <copyright file="FlyoutRegistrySettings.cs" company="">
//   
// </copyright>
// <summary>
//   The flyout registry settings.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace HearthCap.Shell.Flyouts
{
    using System;

    using HearthCap.Shell.UserPreferences;

    using MahApps.Metro.Controls;

    /// <summary>
    /// The flyout registry settings.
    /// </summary>
    public class FlyoutRegistrySettings : RegistrySettings
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FlyoutRegistrySettings"/> class.
        /// </summary>
        public FlyoutRegistrySettings()
            : base(@"Software\HearthstoneTracker\Flyouts")
        {
        }

        /// <summary>
        /// The get position.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="defaultPosition">
        /// The default position.
        /// </param>
        /// <returns>
        /// The <see cref="Position"/>.
        /// </returns>
        public Position GetPosition(Type type, Position defaultPosition = Position.Right)
        {
            return this.GetOrCreate(type.Name, defaultPosition);
        }

        /// <summary>
        /// The get position.
        /// </summary>
        /// <param name="defaultPosition">
        /// The default position.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        /// <returns>
        /// The <see cref="Position"/>.
        /// </returns>
        public Position GetPosition<TType>(Position defaultPosition = Position.Right)
        {
            return this.GetPosition(typeof(TType), defaultPosition);
        }

        /// <summary>
        /// The set position.
        /// </summary>
        /// <param name="type">
        /// The type.
        /// </param>
        /// <param name="position">
        /// The position.
        /// </param>
        public void SetPosition(Type type, Position position)
        {
            this.SetValue(type.Name, position);
        }

        /// <summary>
        /// The set position.
        /// </summary>
        /// <param name="position">
        /// The position.
        /// </param>
        /// <typeparam name="TType">
        /// </typeparam>
        public void SetPosition<TType>(Position position)
        {
            this.SetPosition(typeof(TType), position);
        }
    }
}